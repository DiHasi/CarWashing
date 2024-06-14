using AutoMapper;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class CustomerCarProfile : Profile
{
    public CustomerCarProfile() 
    {
        CreateMap<CustomerCarEntity, CustomerCar>();
        CreateMap<CustomerCar, CustomerCarEntity>();
    }
}