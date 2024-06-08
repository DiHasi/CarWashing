using AutoMapper;
using CarWashing.Domain.ValueObjects;
using CarWashing.Persistence.Entities.ValueObjects;

namespace CarWashing.Profiles;

public class PriceProfile : Profile
{
    public PriceProfile()
    {
        CreateMap<Price, PriceEntity>()
            .ForMember(dest => dest.MaxValue,
                opt => opt.MapFrom(src => src.MaxValue));
            
        CreateMap<PriceEntity, Price>()
            .ConstructUsing(src => Price.Create(src.MaxValue));
    }
}