using CarWashing.Domain.Models;

namespace CarWashing.Persistence.Entities;

public class UserEntity : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }
    public bool IsSendNotify { get; set; }
    
    public List<RoleEntity>? Roles { get; set; }
}