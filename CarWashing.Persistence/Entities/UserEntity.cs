namespace CarWashing.Persistence.Entities;

public class UserEntity : BaseEntity
{
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string FullName => $"{FirstName} {LastName} {Patronymic}".Trim();

    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsSendNotify { get; set; }
    
    public List<RoleEntity>? Roles { get; set; }
}