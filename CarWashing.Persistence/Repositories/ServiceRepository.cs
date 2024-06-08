using AutoFilter;
using AutoMapper;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Context;
using CarWashing.Persistence.Entities;
using CarWashing.Persistence.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CarWashing.Persistence.Repositories;

public class ServiceRepository(CarWashingContext context, IMapper mapper) : IServiceRepository
{
    public async Task<IEnumerable<Service>> GetServices(ServiceFilter filter)
    {
        var query = context.Services
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .AutoFilter(filter);

        if (!string.IsNullOrEmpty(filter.OrderBy))
        {
            var propertyInfo = typeof(Service).GetProperty(filter.OrderBy);
            if (propertyInfo != null)
            {
                query = query.OrderBy(filter.OrderBy);
            }
        }
        
        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var serviceEntities = await query.ToListAsync();
        
        return mapper.Map<IEnumerable<Service>>(serviceEntities);
    }

    public async Task<Service?> GetService(int id)
    {
        var serviceEntity = await context.Services.FindAsync(id) ?? null;
        return mapper.Map<Service>(serviceEntity);
    }

    public async Task<Service> AddService(Service service)
    {
        var addedService = context.Services.Add(mapper.Map<ServiceEntity>(service)).Entity;
        await context.SaveChangesAsync();
        return mapper.Map<Service>(addedService);
    }

    public async Task UpdateService(int id, Service service)
    {
        var serviceEntity = await context.Services.FindAsync(id);
        if (serviceEntity != null)
        {
            serviceEntity.Name = service.Name;
            serviceEntity.Price = new PriceEntity(service.Price.MaxValue);
            serviceEntity.Time = new TimeEntity(service.Time.Minutes);

            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteService(int id)
    {
        var service = await context.Services.FindAsync(id);
        if (service != null) context.Services.Remove(service);
        await context.SaveChangesAsync();
    }
}