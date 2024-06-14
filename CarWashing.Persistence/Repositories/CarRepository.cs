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

public class CarRepository (CarWashingContext context, IMapper mapper): ICarRepository
{
    public async Task<IEnumerable<Car>> GetCars(CarFilter filter)
    {
        var query = context.Cars
            .AsNoTracking()
            .Include(c => c.Brand)
            .OrderBy(c => c.Id)
            .AutoFilter(filter);
        
        if (filter.OrderBy.HasValue)
        {
            var sortBy = filter.OrderBy.Value.GetPath();
            var parameter = Expression.Parameter(typeof(CarEntity), "o");
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
            var lambda = Expression.Lambda<Func<CarEntity, object>>(property, parameter);

            query = filter.ByDescending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }
        
        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var carEntities = await query.ToListAsync();
        
        return mapper.Map<IEnumerable<Car>>(carEntities);
    }

    public async Task<Car?> GetCar(int id)
    {
        var brandEntity = await context.Cars
            .Include(b => b.Brand)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id) ?? null;
        return mapper.Map<Car>(brandEntity);
    }

    public async Task<Car> AddCar(Car car)
    {
        var carEntity = mapper.Map<CarEntity>(car);

        var brandEntity = await context.Brands.FirstOrDefaultAsync(b => b.Name == car.Brand.Name);
        if (brandEntity != null) carEntity.Brand = brandEntity;
        
        var addedCarEntity = context.Cars.Add(carEntity).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<Car>(addedCarEntity);
    }

    public async Task UpdateCar(Car car)
    {
        var carEntity = await context.Cars
            .Include(c => c.Brand)
            .FirstOrDefaultAsync(c => c.Id == car.Id);
        
        if (carEntity != null)
        {
            carEntity.Model = car.Model;

            var brandEntity = await context.Brands.FirstOrDefaultAsync(b => b.Name == car.Brand.Name);
            if (brandEntity != null) carEntity.Brand = brandEntity;
            
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteCar(int id)
    {
        var car = await context.Cars.FindAsync(id);
        if (car != null) context.Cars.Remove(car);
        await context.SaveChangesAsync();
    }
}