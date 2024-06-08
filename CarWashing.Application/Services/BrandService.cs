using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;

namespace CarWashing.Application.Services;

public class BrandService(IBrandRepository brandRepository)
{
    public async Task<IEnumerable<Brand>> GetBrands(BrandFilter filter)
    {
        return await brandRepository.GetBrands(filter);
    }

    public async Task<Brand?> GetBrand(int id)
    {
        return await brandRepository.GetBrand(id);
    }

    public async Task<Brand> AddBrand(Brand service)
    {
        return await brandRepository.AddBrand(service);
    }

    public async Task UpdateBrand(int id, Brand service)
    {
        await brandRepository.UpdateBrand(id, service);
    }

    public async Task DeleteBrand(int id)
    {
        await brandRepository.DeleteBrand(id);
    }
}