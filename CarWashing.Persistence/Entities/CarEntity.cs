using CarWashing.Persistence.Entities;

namespace CarWashing.Domain.Models;

public class CarEntity
{
    public int Id { get; set; }
    public string Model { get; set; }
    public BrandEntity Brand { get; set; }
}