using CarWashing.Application.Interfaces.Auth;
using CarWashing.Application.Services;
using CarWashing.Infrastructure;

namespace CarWashing.Configurations;

public static class ServiceConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceService>();
        services.AddScoped<BrandService>();
        services.AddScoped<CarService>();
        services.AddScoped<UserService>();
        services.AddScoped<CustomerCarService>();
        services.AddScoped<OrderService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
    }
}