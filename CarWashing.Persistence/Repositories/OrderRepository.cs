using System.Linq.Expressions;
using AutoFilter;
using AutoMapper;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Context;
using CarWashing.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashing.Persistence.Repositories;

public class OrderRepository(CarWashingContext context, IMapper mapper) : IOrderRepository
{
    public async Task<IEnumerable<Order>> GetOrders(OrderFilter filter)
    {
        var query = context.Orders
            .Include(o => o.Employee)
            .Include(o => o.Administrator)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Customer)
            .Include(o => o.Services)
            .AsNoTracking()
            .OrderBy(b => b.Id)
            .AutoFilter(filter);
        
        if (filter.OrderBy.HasValue)
        {
            var sortBy = filter.OrderBy.Value.GetPath();
            var parameter = Expression.Parameter(typeof(OrderEntity), "o");
            Expression property;
            if (sortBy.Contains('.'))
            {
                var parts = sortBy.Split('.');
                property = Expression.Property(parameter, parts[0]);
                for (var i = 1; i < parts.Length; i++)
                {
                    property = Expression.Property(property, parts[i]);
                }
            }
            else
            {
                property = Expression.Property(parameter, sortBy);
            }
            var lambda = Expression.Lambda<Func<OrderEntity, object>>(property, parameter);

            query = filter.ByDescending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }
        

        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var orderEntities = await query.ToListAsync();

        return mapper.Map<IEnumerable<Order>>(orderEntities);
    }

    public async Task<Order?> GetOrder(int id)
    {
        var orderEntity = await context.Orders
            .Include(o => o.Employee)
            .Include(o => o.Administrator)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Customer)
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Id == id) ?? null;
        return mapper.Map<Order>(orderEntity);
    }

    public async Task<Order> AddOrder(Order order)
    {
        var addedOrder = context.Orders.Add(mapper.Map<OrderEntity>(order)).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<Order>(addedOrder);
    }

    public async Task UpdateOrder(Order order)
    {
        var orderEntity = await context.Orders
            .Include(o => o.Employee)
            .Include(o => o.Administrator)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Customer)
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        if (orderEntity != null)
        {
            var administratorEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == order.Administrator.Id);
            if(administratorEntity != null) orderEntity.Administrator = administratorEntity;
            
            var employeeEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == order.Employee.Id);
            if(employeeEntity != null) orderEntity.Employee = employeeEntity;
            
            var customerCarEntity = await context.CustomerCars
                .Include(c => c.Car)
                .ThenInclude(c => c.Brand)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == order.CustomerCar.Id);
            
            if (customerCarEntity != null) orderEntity.CustomerCar = customerCarEntity;

            orderEntity.Status = order.Status;
            orderEntity.StartDate = order.StartDate;
            orderEntity.EndDate = order.EndDate;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteOrder(int id)
    {
        var order = await context.Orders
            .Include(o => o.Employee)
            .Include(o => o.Administrator)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(o => o.CustomerCar)
            .ThenInclude(c => c.Customer)
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Id == id) ?? null;

        if (order != null) context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }
    
    public async Task AddServices(int id, List<Service> services)
    {
        var order = await context.Orders
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Id == id);

        mapper.Map<Order>(order).AddServices(services);
        
        await context.SaveChangesAsync();
    }

    public async Task CompleteOrder(int id)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            order.Status = Status.Completed;
            await context.SaveChangesAsync();
        }
    }
}