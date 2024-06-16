using System.Linq.Expressions;
using AutoFilter;
using AutoMapper;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Context;
using CarWashing.Persistence.Entities;
using CSharpFunctionalExtensions;
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

            var converted = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<OrderEntity, object>>(converted, parameter);

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
        var orderEntity = mapper.Map<OrderEntity>(order);

        var adminEntity = context.Users.Local.FirstOrDefault(u => u.Id == orderEntity.Administrator.Id)
                          ?? await context.Users.AsNoTracking()
                              .FirstOrDefaultAsync(u => u.Id == orderEntity.Administrator.Id);

        if (adminEntity != null)
        {
            context.Entry(adminEntity).State = EntityState.Detached;
            context.Attach(adminEntity);
            orderEntity.Administrator = adminEntity;
        }

        var employeeEntity = context.Users.Local.FirstOrDefault(u => u.Id == orderEntity.Employee.Id)
                             ?? await context.Users.AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Id == orderEntity.Employee.Id);

        if (employeeEntity != null)
        {
            context.Entry(employeeEntity).State = EntityState.Detached;
            context.Attach(employeeEntity);
            orderEntity.Employee = employeeEntity;
        }

        var customerCarEntity = context.CustomerCars.Local.FirstOrDefault(c => c.Id == orderEntity.CustomerCar.Id)
                                ?? await context.CustomerCars.AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == orderEntity.CustomerCar.Id);

        if (customerCarEntity != null)
        {
            context.Entry(customerCarEntity).State = EntityState.Detached;
            context.Attach(customerCarEntity);
            orderEntity.CustomerCar = customerCarEntity;
        }

        if (orderEntity.Services != null)
        {
            for (var i = 0; i < orderEntity.Services.Count; i++)
            {
                var existingService = context.Services.Local.FirstOrDefault(s => s.Id == orderEntity.Services[i].Id)
                                      ?? await context.Services.AsNoTracking()
                                          .FirstOrDefaultAsync(s => s.Id == orderEntity.Services[i].Id);

                if (existingService == null) continue;
                context.Entry(existingService).State = EntityState.Detached;
                context.Attach(existingService);
                orderEntity.Services[i] = existingService;
            }
        }

        var addedOrder = context.Orders.Add(orderEntity).Entity;
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
            if (administratorEntity != null) orderEntity.Administrator = administratorEntity;

            var employeeEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == order.Employee.Id);
            if (employeeEntity != null) orderEntity.Employee = employeeEntity;

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

    public async Task<Result<Order>> AddServices(int id, List<Service> services)
    {
        var orderEntity = await context.Orders
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Id == id);

        var order = mapper.Map<Order>(orderEntity);

        var result = order.AddServices(services);

        if (result.IsFailure) return Result.Failure<Order>(result.Error);

        if (orderEntity == null) return Result.Success(order);
        var serviceEntities = mapper.Map<List<ServiceEntity>>(order.Services);

        foreach (var serviceEntity in serviceEntities)
        {
            var existingEntity = context.Services.Local.FirstOrDefault(s => s.Id == serviceEntity.Id);
            if (existingEntity != null)
            {
                context.Entry(existingEntity).State = EntityState.Detached;
            }

            if (orderEntity.Services != null && orderEntity.Services.All(s => s.Id != serviceEntity.Id))
            {
                orderEntity.Services?.Add(serviceEntity);
            }
        }

        await context.SaveChangesAsync();

        return Result.Success(order);
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