using AutoMapper;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;

namespace CarWashing.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderEntity>();
        CreateMap<OrderEntity, Order>();
    }
}