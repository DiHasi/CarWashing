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

        if (!string.IsNullOrEmpty(filter.OrderBy))
        {
            var propertyInfo = typeof(Brand).GetProperty(filter.OrderBy);
            if (propertyInfo != null)
            {
                query = query.OrderBy(filter.OrderBy);
            }
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

    public async Task UpdateBrand(int id, Brand service)
    {
        var brandEntity = await context.Brands.FindAsync(id);
        if (brandEntity != null)
        {
            brandEntity.Name = service.Name;
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