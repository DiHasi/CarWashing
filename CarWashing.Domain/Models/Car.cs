namespace CarWashing.Domain.Models;

public class Car
{
    public int Id { get; set; }
    public string Model { get; set; }
    public virtual Brand Brand { get; set; }
}