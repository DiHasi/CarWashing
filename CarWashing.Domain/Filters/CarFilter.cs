namespace CarWashing.Domain.Filters;

public class CarFilter : Filter
{
    public string? Model { get; set; }
    public string? Brand { get; set; }
}