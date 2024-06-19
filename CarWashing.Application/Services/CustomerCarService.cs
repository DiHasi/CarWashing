using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CSharpFunctionalExtensions;

namespace CarWashing.Application.Services;

public class CustomerCarService(ICustomerCarRepository customerCarRepository,
    ICarRepository carRepository,
    IUserRepository userRepository
)
{
    public async Task<IEnumerable<CustomerCar>> GetCustomerCars(CustomerCarFilter filter)
    {
        return await customerCarRepository.GetCustomerCars(filter);
    }

    public async Task<CustomerCar?> GetCustomerCar(int id)
    {
        return await customerCarRepository.GetCustomerCar(id);
    }

    public async Task<Result<CustomerCar>> AddCustomerCar(int carId, int userId, int year, string number)
    {
        var car = await carRepository.GetCar(carId);
        if (car == null) return Result.Failure<CustomerCar>("Car not found");
        
        var user = await userRepository.GetUser(userId);
        if (user == null) return Result.Failure<CustomerCar>("User not found");
        if (!user.Roles.Contains(Role.User)) return Result.Failure<CustomerCar>("User is not customer");
        
        var customerCar = CustomerCar.Create(car, user, year, number);
        if (customerCar.IsFailure) return Result.Failure<CustomerCar>(customerCar.Error);
        
        return await customerCarRepository.AddCustomerCar(customerCar.Value);

    }

    public async Task<Result<CustomerCar>> UpdateCustomerCar(int id, int carId, int userId, int year, string number)
    {
        var customerCar = await customerCarRepository.GetCustomerCar(id);
        if (customerCar == null) return Result.Failure<CustomerCar>("CustomerCar not found");

        var car = await carRepository.GetCar(carId);
        if (car == null) return Result.Failure<CustomerCar>("Car not found");
        
        var user = await userRepository.GetUser(userId);
        if (user == null) return Result.Failure<CustomerCar>("User not found");
        if (!user.Roles.Contains(Role.User)) return Result.Failure<CustomerCar>("User is not customer");

        var result = customerCar.ChangeYear(year);
        if (result.IsFailure) return Result.Failure<CustomerCar>(result.Error);
        
        result = customerCar.ChangeNumber(number);
        if (result.IsFailure) return Result.Failure<CustomerCar>(result.Error);
        
        result = customerCar.ChangeCustomer(user);
        if (result.IsFailure) return Result.Failure<CustomerCar>(result.Error);
        
        result = customerCar.ChangeCar(car);
        if (result.IsFailure) return Result.Failure<CustomerCar>(result.Error);
        
        await customerCarRepository.UpdateCustomerCar(customerCar);
        
        return Result.Success(customerCar);
    }

    public async Task DeleteCustomerCar(int id)
    {
        await customerCarRepository.DeleteCustomerCar(id);
    }
}