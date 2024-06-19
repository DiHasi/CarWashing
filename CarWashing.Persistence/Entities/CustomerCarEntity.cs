using CarWashing.Domain.Models;

namespace CarWashing.Persistence.Entities;

public class CustomerCarEntity : BaseEntity
{
    public CarEntity Car { get; set; }
    public UserEntity Customer { get; set; }
    public int Year { get; set; }
    public string Number { get; set; } = string.Empty;
}