using AutoMapper;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => (src.Roles ?? new List<RoleEntity>()).Select(r => (Role)Enum.Parse(typeof(Role), r.Name))));
        CreateMap<User, UserEntity>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => new RoleEntity { Id = (int)r, Name = r.ToString() })));
    }
}