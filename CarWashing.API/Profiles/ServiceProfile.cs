using AutoMapper;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<ServiceEntity, Service>();
        CreateMap<Service, ServiceEntity>();
    }
}