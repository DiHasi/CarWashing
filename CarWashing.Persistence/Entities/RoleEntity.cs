using CarWashing.Domain.Models;

namespace CarWashing.Persistence.Entities;

public class RoleEntity : BaseEntity
{
    public string Name { get; set; }

    public List<UserEntity>? Users { get; set; }
}