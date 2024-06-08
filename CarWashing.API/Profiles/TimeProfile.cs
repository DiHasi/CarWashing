using AutoMapper;
using CarWashing.Domain.ValueObjects;
using CarWashing.Persistence.Entities.ValueObjects;

namespace CarWashing.Profiles;

public class TimeProfile : Profile
{
    public TimeProfile()
    {
        CreateMap<Time, TimeEntity>()
            .ForMember(dest => dest.Minutes,
                opt => opt.MapFrom(src => src.Minutes));

        CreateMap<TimeEntity, Time>()
            .ConstructUsing(src => Time.Create(src.Minutes));
    }
}