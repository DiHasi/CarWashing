using AutoMapper;
using CarWashing.Domain.Enums;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<RoleEntity, Role>()
            .ConvertUsing(src => (Role)Enum.Parse(typeof(Role), src.Name));
        CreateMap<Role, RoleEntity>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToString()));
    }
}