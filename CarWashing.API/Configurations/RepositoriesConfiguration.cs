using CarWashing.Domain.Interfaces;
using CarWashing.Persistence.Repositories;

namespace CarWashing.Configurations;

public static class RepositoriesConfiguration
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerCarRepository, CustomerCarRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}