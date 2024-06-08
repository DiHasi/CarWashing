using CarWashing.Profiles;

namespace CarWashing.Configurations;

public static class AutoMapperConfiguration
{
    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(ServiceProfile), 
            typeof(PriceProfile), 
            typeof(TimeProfile), 
            typeof(BrandProfile),
            typeof(CarProfile));
    }
}