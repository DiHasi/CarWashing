using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class Brand
{
    private Brand(string name)
    {
        Name = name;
    }
    public int Id { get; private set; }
    public string Name { get; private set; }
    
    public static Result<Brand> Create(string name)
    {
        if (string.IsNullOrEmpty(name)) return Result.Failure<Brand>("Name cannot be empty");
        if (name.Length > 100) return Result.Failure<Brand>("Name cannot be longer than 100 characters");
        
        var newBrand = new Brand(name);
        
        return Result.Success(newBrand);
    }
    
    public Result<Brand> ChangeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return Result.Failure<Brand>("Name cannot be empty");
        if (name.Length > 100) return Result.Failure<Brand>("Name cannot be longer than 100 characters");
        
        Name = name;
        return Result.Success(this);
    }
}