using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Repositories;

namespace CarWashing.Application.Services;

public class ServiceService(IServiceRepository serviceRepository)
{
    public async Task<IEnumerable<Service>> GetServices(ServiceFilter filter)
    {
        return await serviceRepository.GetServices(filter);
    }

    public async Task<Service?> GetService(int id)
    {
        return await serviceRepository.GetService(id);
    }

    public async Task<Service> AddService(Service service)
    {
        return await serviceRepository.AddService(service);
    }

    public async Task UpdateService(int id, Service service)
    {
        await serviceRepository.UpdateService(id, service);
    }

    public async Task DeleteService(int id)
    {
        await serviceRepository.DeleteService(id);
    }
}