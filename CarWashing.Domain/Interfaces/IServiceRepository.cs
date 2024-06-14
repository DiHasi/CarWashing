using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;

namespace CarWashing.Domain.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetServices(ServiceFilter filter);
    Task<IEnumerable<Service>> GetServices(List<int> servicesIds);
    Task<Service?> GetService(int id);
    Task<Service> AddService(Service service);
    Task UpdateService(Service service);
    Task DeleteService(int id);
}