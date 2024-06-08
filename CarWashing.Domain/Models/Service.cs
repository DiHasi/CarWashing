using CarWashing.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class Service
{
    private Service(string name, Price price, Time time)
    {
        Name = name;
        Price = price;
        Time = time;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public Price Price { get; private set; }
    
    public Time Time { get; private set;}
    
    public static Result<Service> Create(string name, Price price, Time time)
    {
        if(string.IsNullOrWhiteSpace(name)) return Result.Failure<Service>("Name is required");
        
        if(price.MaxValue <= 0) return Result.Failure<Service>("Price must be positive");
        
        if(time.Minutes <= 0) return Result.Failure<Service>("Time must be positive");
        
        var service = new Service(name, price, time);

        return Result.Success(service);
    }
    
    public Result<Service> ChangeName(string name)
    {
        if(string.IsNullOrWhiteSpace(name)) return Result.Failure<Service>("Name is required");
        if(name.Length > 100) return Result.Failure<Service>("Name cannot be longer than 100 characters");
        
        Name = name;
        
        return Result.Success(this);
    }
    
    public Result<Service> ChangePrice(Price price)
    {
        if(price.MaxValue <= 0) return Result.Failure<Service>("Price must be positive");
        
        Price = price;
        
        return Result.Success(this);
    }
    
    public Result<Service> ChangeTime(Time time)
    {
        if(time.Minutes <= 0) return Result.Failure<Service>("Time must be positive");
        
        Time = time;
        
        return Result.Success(this);
    }
}