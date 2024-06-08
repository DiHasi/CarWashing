using AutoMapper;
using CarWashing.Domain.Models;

namespace CarWashing.Profiles;

public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<CarEntity, Car>();
        CreateMap<Car, CarEntity>();
    }
}