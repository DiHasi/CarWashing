using CarWashing.Persistence.Entities.ValueObjects;

namespace CarWashing.Persistence.Entities;

public class ServiceEntity : BaseEntity
{
    public string Name { get; set; }
    public PriceEntity Price { get; set; }
    public TimeEntity Time { get; set;}
    public List<OrderEntity>? Orders { get; set; }
}