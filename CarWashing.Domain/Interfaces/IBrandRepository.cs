using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;

namespace CarWashing.Domain.Interfaces;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> GetBrands(BrandFilter filter);
    Task<Brand?> GetBrand(int id);
    Task<Brand> AddBrand(Brand brand);
    Task UpdateBrand(int id, Brand service);
    Task DeleteBrand(int id);
}