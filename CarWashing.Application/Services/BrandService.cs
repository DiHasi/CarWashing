using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CSharpFunctionalExtensions;

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

    public async Task<Result<Brand>> AddBrand(string name)
    {
        var brand = Brand.Create(name);
        if (brand.IsFailure) return Result.Failure<Brand>(brand.Error);
        return await brandRepository.AddBrand(brand.Value);
    }

    public async Task<Result<Brand>> UpdateBrand(int id, string name)
    {
        var brand = await brandRepository.GetBrand(id);
        if (brand == null)
        {
            return Result.Failure<Brand>("Brand not found");
        }
        
        brand.ChangeName(name);
        
        await brandRepository.UpdateBrand(brand);
        
        return Result.Success(brand);
    }

    public async Task DeleteBrand(int id)
    {
        await brandRepository.DeleteBrand(id);
    }
}