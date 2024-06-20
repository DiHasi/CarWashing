using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;
using CSharpFunctionalExtensions;

namespace CarWashing.Application.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    ICustomerCarRepository customerCarRepository,
    IServiceRepository serviceRepository)
{
    public async Task<Result<IEnumerable<Order>>> GetOrders(OrderFilter filter)
    {
        var orders = await orderRepository.GetOrders(filter);
        
        return Result.Success(orders);
    }

    public async Task<Order?> GetOrder(int id)
    {
        return await orderRepository.GetOrder(id);
    }

    public async Task<Result<Order>> AddOrder(int administratorId, int employeeId, int customerCarId, List<int> serviceIds)
    {
        var administrator = await userRepository.GetUser(administratorId);
        if (administrator == null) return Result.Failure<Order>("Administrator not found");
        if (!administrator.Roles.Contains(Role.Administrator)) return Result.Failure<Order>("Wrong administrator");

        var employee = await userRepository.GetUser(employeeId);
        if (employee == null) return Result.Failure<Order>("Employee not found");
        if (!employee.Roles.Contains(Role.Employee)) return Result.Failure<Order>("Wrong employee");

        var customerCar = await customerCarRepository.GetCustomerCar(customerCarId);
        if (customerCar == null) return Result.Failure<Order>("Customer car not found");

        var services = await serviceRepository.GetServices(serviceIds);

        var order = Order.Create(administrator, employee, customerCar, services.ToList());
        if (order.IsFailure) return Result.Failure<Order>(order.Error);

        return await orderRepository.AddOrder(order.Value);
    }

    public async Task<Result<Order>> UpdateOrder(int id, int administratorId, int employeeId, int customerCarId)
    {
        var order = await orderRepository.GetOrder(id);
        if (order == null) return Result.Failure<Order>("Order not found");

        var administrator = await userRepository.GetUser(administratorId);
        if (administrator == null) return Result.Failure<Order>("Administrator not found");
        if (!administrator.Roles.Contains(Role.Administrator)) return Result.Failure<Order>("Wrong administrator");
        
        var employee = await userRepository.GetUser(employeeId);
        if (employee == null) return Result.Failure<Order>("Employee not found");
        if (!employee.Roles.Contains(Role.Employee)) return Result.Failure<Order>("Wrong employee");

        var customerCar = await customerCarRepository.GetCustomerCar(customerCarId);
        if (customerCar == null) return Result.Failure<Order>("Customer car not found");
        
        if (order.Status == Status.Completed) return Result.Failure<Order>("Order already completed");

        var result = order.ChangeAdministrator(administrator);
        if (result.IsFailure) return Result.Failure<Order>(result.Error);
        
        result = order.ChangeEmployee(employee);
        if (result.IsFailure) return Result.Failure<Order>(result.Error);
        
        result = order.ChangeCustomerCar(customerCar);
        if (result.IsFailure) return Result.Failure<Order>(result.Error);
        
        await orderRepository.UpdateOrder(order);

        return Result.Success(order);
    }

    public async Task DeleteOrder(int id)
    {
        await orderRepository.DeleteOrder(id);
    }

    public async Task<Result<Order>> AddServices(int id, List<int> serviceIds)
    {
        var order = await orderRepository.GetOrder(id);
        if (order == null) return Result.Failure<Order>("Order not found");
        if(order.Status == Status.Completed) return Result.Failure<Order>("Order already completed");
        
        var newServices = await serviceRepository.GetServices(serviceIds);

        var result = await orderRepository.AddServices(id, newServices.ToList());
        
        return result.IsFailure ? Result.Failure<Order>(result.Error) : Result.Success(order);
    }
    
    public async Task<Result<Order>> CompleteOrder(int id)
    {
        var order = await orderRepository.GetOrder(id);
        if (order == null) return Result.Failure<Order>("Order not found");
        if(order.Status == Status.Completed) return Result.Failure<Order>("Order already completed");
        
        await orderRepository.CompleteOrder(id);
        
        return Result.Success(order);
    }
}