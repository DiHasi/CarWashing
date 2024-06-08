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

        if (!string.IsNullOrEmpty(filter.OrderBy))
        {
            var propertyInfo = typeof(Car).GetProperty(filter.OrderBy);
            if (propertyInfo != null)
            {
                query = query.OrderBy(filter.OrderBy);
            }
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
        var addedBrand = context.Cars.Add(mapper.Map<CarEntity>(car)).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<Car>(addedBrand);
    }

    public async Task UpdateCar(int id, Car car)
    {
        var carEntity = await context.Cars.FindAsync(id);
        if (carEntity != null)
        {
            carEntity.Model = car.Model;
            carEntity.Brand = new BrandEntity{Name = car.Brand.Name};
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