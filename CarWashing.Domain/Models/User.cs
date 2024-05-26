using System.Collections;

namespace CarWashing.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }
    public bool IsSendNotify { get; set; }
    public List<Role> Roles { get; set; }
}