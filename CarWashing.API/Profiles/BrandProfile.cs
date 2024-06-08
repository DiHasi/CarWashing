using AutoMapper;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class BrandProfile : Profile
{
    public BrandProfile() 
    {
        CreateMap<BrandEntity, Brand>();
        CreateMap<Brand, BrandEntity>();
    }
}