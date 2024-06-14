using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class CustomerCar
{
    private CustomerCar(Car car, User customer, int year, string number)
    {
        Car = car;
        Customer = customer;
        Year = year;
        Number = number;
    }

    public int Id { get; private set; }
    public Car Car { get; private set; }
    public User Customer { get; private set; }
    public int Year { get; private set; }
    public string Number { get; private set; }
    
    public static Result<CustomerCar> Create(Car car, User customer, int year, string number)
    {
        if (year < 0) return Result.Failure<CustomerCar>("Year cannot be negative");
        if (string.IsNullOrWhiteSpace(number)) return Result.Failure<CustomerCar>("Number cannot be empty");
        
        var customerCar = new CustomerCar(car, customer, year, number);
        
        return Result.Success(customerCar);
    }

    public Result<CustomerCar> ChangeCar(Car car)
    {
        Car = car;
        return Result.Success(this);
    }
    
    public Result<CustomerCar> ChangeCustomer(User customer)
    {
        Customer = customer;
        return Result.Success(this);
    }
    
    public Result<CustomerCar> ChangeYear(int year)
    {
        Year = year;
        return Result.Success(this);
    }
    
    public Result<CustomerCar> ChangeNumber(string number)
    {
        Number = number;
        return Result.Success(this);
    } 
}