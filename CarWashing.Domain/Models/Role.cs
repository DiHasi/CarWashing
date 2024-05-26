namespace CarWashing.Domain.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<User> Users { get; set; }
}