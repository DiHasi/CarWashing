using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace CarWashing.Application.Services;

public class ServiceService(IServiceRepository serviceRepository)
{
    public async Task<Result<IEnumerable<Service>>> GetServices(ServiceFilter filter)
    {
        var services = await serviceRepository.GetServices(filter);

        return Result.Success(services);
    }

    public async Task<Service?> GetService(int id)
    {
        return await serviceRepository.GetService(id);
    }

    public async Task<Result<Service>> AddService(string name, int price, int time)
    {
        var service = Service.Create(name, price, time);
        if (service.IsFailure) return Result.Failure<Service>(service.Error);
        return await serviceRepository.AddService(service.Value);
    }

    public async Task<Result<Service>> UpdateService(int id, string name, int price, int time)
    {
        var serviceToUpdate = await GetService(id);
        if (serviceToUpdate == null) return Result.Failure<Service>("Service not found");

        var result = serviceToUpdate.ChangeName(name);
        if (result.IsFailure) return Result.Failure<Service>(result.Error);

        result = result.Value.ChangePrice(Price.Create(price));
        if (result.IsFailure) return Result.Failure<Service>(result.Error);

        result = result.Value.ChangeTime(Time.Create(time));
        if (result.IsFailure) return Result.Failure<Service>(result.Error);

        await serviceRepository.UpdateService(result.Value);

        return Result.Success(result.Value);
    }

    public async Task DeleteService(int id)
    {
        await serviceRepository.DeleteService(id);
    }
}