namespace CarWashing.Domain.Models;

public class Order
{
    public int Id { get; set; }
    public User Administrator { get; set; }
    public User Employee { get; set; }
    public CustomerCar CustomerCar { get; set; }
    public int Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public List<Service> Services { get; set; }
}