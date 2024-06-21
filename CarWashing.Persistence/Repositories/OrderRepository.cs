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
    public async Task<IEnumerable<Order>> GetOrders(OrderFilter filter, bool isOnlyUser, int userId)
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
            .AsNoTracking();

        if (isOnlyUser)
        {
            query = query.Where(o => o.CustomerCar.Customer.Id == userId);
        }

        query = filter.ByDescending ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id);
        query = query.AutoFilter(filter);
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
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id) ?? null;
        return mapper.Map<Order>(orderEntity);
    }

    public async Task<Order> AddOrder(Order order)
    {
        var orderEntity = mapper.Map<OrderEntity>(order);

        context.Entry(orderEntity).State = EntityState.Unchanged;

        var addedOrder = context.Orders.Add(orderEntity).Entity;
        if (orderEntity.Services != null)
            foreach (var serviceEntity in orderEntity.Services)
            {
                context.Entry(serviceEntity).State = EntityState.Modified;
            }

        await context.SaveChangesAsync();

        return mapper.Map<Order>(addedOrder);
    }


    public async Task UpdateOrder(Order order)
    {
        var orderEntity = await context.Orders
            .Include(o => o.Employee)
            .Include(o => o.Administrator)
            .Include(o => o.CustomerCar)
            .Include(o => o.Services)
            .SingleAsync(o => o.Id == order.Id);

        // context.Entry(orderEntity).CurrentValues.SetValues(order);
        
        var newOrderEntity = mapper.Map<OrderEntity>(order);
        
        if (orderEntity.Employee.Id != newOrderEntity.Employee.Id)
        {
            orderEntity.Employee = newOrderEntity.Employee;
            context.Entry(orderEntity.Employee).State = EntityState.Modified;
        }
        
        if (orderEntity.Administrator.Id != newOrderEntity.Administrator.Id)
        {
            orderEntity.Administrator = newOrderEntity.Administrator;
            context.Entry(orderEntity.Administrator).State = EntityState.Modified;
        }
        
        if (orderEntity.CustomerCar.Id != newOrderEntity.CustomerCar.Id)
        {
            orderEntity.CustomerCar = newOrderEntity.CustomerCar;
            context.Entry(orderEntity.CustomerCar).State = EntityState.Modified;
        }

        await context.SaveChangesAsync();
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