using System.Linq.Expressions;
using AutoFilter;
using AutoMapper;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Context;
using CarWashing.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashing.Persistence.Repositories;

public class CustomerCarRepository(CarWashingContext context, IMapper mapper) : ICustomerCarRepository
{
    public async Task<IEnumerable<CustomerCar>> GetCustomerCars(CustomerCarFilter filter)
    {
        var query = context.CustomerCars
            .Include(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(c => c.Customer)
            .AsNoTracking()
            .OrderBy(b => b.Id)
            .AutoFilter(filter);

        if (filter.OrderBy.HasValue)
        {
            var sortBy = filter.OrderBy.Value.GetPath();
            var parameter = Expression.Parameter(typeof(CustomerCarEntity), "o");
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
            var lambda = Expression.Lambda<Func<CustomerCarEntity, object>>(property, parameter);

            query = filter.ByDescending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }
        
        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var customerCarEntities = await query.ToListAsync();
        
        return mapper.Map<IEnumerable<CustomerCar>>(customerCarEntities);
    }

    public async Task<CustomerCar?> GetCustomerCar(int id)
    {
        var customerCarEntity = await context.CustomerCars
            .Include(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id) ?? null;
        return mapper.Map<CustomerCar>(customerCarEntity);
    }

    public async Task<CustomerCar> AddCustomerCar(CustomerCar customerCar)
    {
        var customerCarEntity = mapper.Map<CustomerCarEntity>(customerCar);

        var carEntity = await context.Cars.FindAsync(customerCar.Car.Id);
        if (carEntity != null)
        {
            customerCarEntity.Car = carEntity;
        }
        
        var brandEntity = await context.Brands.FirstOrDefaultAsync(b => b.Name == customerCar.Car.Brand.Name);
        if (brandEntity != null)
        {
            customerCarEntity.Car.Brand = brandEntity;
        }
        
        var customerEntity = await context.Users.FirstOrDefaultAsync(u => u.Email == customerCar.Customer.Email);
        if (customerEntity != null)
        {
            customerCarEntity.Customer = customerEntity;
        }
        

        var addedCustomerCarEntity = context.CustomerCars.Add(customerCarEntity).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<CustomerCar>(addedCustomerCarEntity);
    }

    public async Task UpdateCustomerCar(CustomerCar customerCar)
    {
        var customerCarEntity = await context.CustomerCars
            .Include(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == customerCar.Id) ?? null;
        
        if (customerCarEntity != null)
        {
            var carEntity = await context.Cars
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.Id == customerCar.Car.Id);
            
            if (carEntity != null) customerCarEntity.Car = carEntity;

            var customerEntity = await context.Users.FirstOrDefaultAsync(u => u.Email == customerCar.Customer.Email);
            if (customerEntity != null) customerCarEntity.Customer = customerEntity;

            customerCarEntity.Car.Model = customerCar.Car.Model;
            customerCarEntity.Year = customerCar.Year;
            customerCarEntity.Number = customerCar.Number;
            
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteCustomerCar(int id)
    {
        var customerCar = await context.CustomerCars
            .Include(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id) ?? null;
        if (customerCar != null) context.CustomerCars.Remove(customerCar);
        await context.SaveChangesAsync();
    }
}