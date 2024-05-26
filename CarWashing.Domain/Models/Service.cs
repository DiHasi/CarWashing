namespace CarWashing.Domain.Models;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public DateTime Time { get; set; }
    
    public List<Order> Orders { get; set; }
}