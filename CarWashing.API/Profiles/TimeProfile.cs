using AutoMapper;
using CarWashing.Domain.ValueObjects;
using CarWashing.Persistence.Entities.ValueObjects;

namespace CarWashing.Profiles;

public class TimeProfile : Profile
{
    public TimeProfile()
    {
        CreateMap<Time, TimeEntity>()
            .ForMember(dest => dest.Seconds,
                opt => opt.MapFrom(src => src.Seconds));

        CreateMap<TimeEntity, Time>()
            .ConstructUsing(src => Time.Create(src.Seconds / 60));
    }
}