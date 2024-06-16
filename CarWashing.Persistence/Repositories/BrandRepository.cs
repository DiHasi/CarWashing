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

public class BrandRepository(CarWashingContext context, IMapper mapper) : IBrandRepository
{
    public async Task<IEnumerable<Brand>> GetBrands(BrandFilter filter)
    {
        var query = context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Id)
            .AutoFilter(filter);
        
        if (filter.OrderBy.HasValue)
        {
            var sortBy = filter.OrderBy.Value.GetPath();
            var parameter = Expression.Parameter(typeof(BrandEntity), "o");
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
            var lambda = Expression.Lambda<Func<BrandEntity, object>>(converted, parameter);

            query = filter.ByDescending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }

        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var brandEntities = await query.ToListAsync();
        
        return mapper.Map<IEnumerable<Brand>>(brandEntities);
    }

    public async Task<Brand?> GetBrand(int id)
    {
        var brandEntity = await context.Brands.FindAsync(id) ?? null;
        return mapper.Map<Brand>(brandEntity);
    }

    public async Task<Brand> AddBrand(Brand brand)
    {
        var addedBrand = context.Brands.Add(mapper.Map<BrandEntity>(brand)).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<Brand>(addedBrand);
    }

    public async Task UpdateBrand(Brand brand)
    {
        var brandEntity = await context.Brands.FindAsync(brand.Id);
        if (brandEntity != null)
        {
            brandEntity.Name = brand.Name;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteBrand(int id)
    {
        var brand = await context.Brands.FindAsync(id);
        if (brand != null) context.Brands.Remove(brand);
        await context.SaveChangesAsync();
    }
}