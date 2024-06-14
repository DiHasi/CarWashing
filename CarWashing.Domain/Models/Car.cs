using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class Car
{
    private Car(string model, Brand brand)
    {
        Model = model;
        Brand = brand;
    }
    public int Id { get; private set; }
    public string Model { get; private set; }
    public Brand Brand { get; private set; }
    
    public static Result<Car> Create(string model, Brand brand)
    {
        return new Car(model, brand);
    }
    
    public Result<Car> ChangeModel(string model)
    {
        if (model.Length > 100) return Result.Failure<Car>("Model cannot be longer than 100 characters");
        
        Model = model;

        return Result.Success(this);
    }

    public Result<Car> ChangeBrand(Brand brand)
    {
        Brand = brand;
        return Result.Success(this);
    }
}