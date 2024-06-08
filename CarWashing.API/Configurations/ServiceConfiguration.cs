using CarWashing.Application.Services;

namespace CarWashing.Configurations;

public static class ServiceConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceService>();
        services.AddScoped<BrandService>();
        services.AddScoped<CarService>();
    }
}