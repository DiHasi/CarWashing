namespace CarWashing.Domain.Models;

public class CustomerCar
{
    public int Id { get; set; }
    public Car Car { get; set; }
    public User Customer { get; set; }
    public int Year { get; set; }
    public string Number { get; set; }
}