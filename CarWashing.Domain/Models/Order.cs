using CarWashing.Domain.Enums;
using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class Order
{
    private Order(User administrator, User employee, CustomerCar customerCar, List<Service> services)
    {
        Administrator = administrator;
        Employee = employee;
        CustomerCar = customerCar;
        _services.AddRange(services);
    }

    public int Id { get; private set; }
    public User Administrator { get; private set; }
    public User Employee { get; private set; }
    public CustomerCar CustomerCar { get; private set; }
    public Status Status { get; private set; } = Status.InProgress;
    public DateTime StartDate { get; private set; } = DateTime.UtcNow;
    public DateTime EndDate => StartDate.AddMinutes(_services.Sum(s => s.Time.Minutes));

    public int TotalPrice => _services.Sum(s => s.Price.MaxValue);

    public int TotalTime => _services.Sum(s => s.Time.Minutes);

    private readonly List<Service> _services = [];

    public IReadOnlyList<Service> Services => _services;

    public static Result<Order> Create(User administrator, User employee, CustomerCar customerCar,
        List<Service> services)
    {
        if (services.Count == 0) return Result.Failure<Order>("Order must have at least one service");

        var order = new Order(administrator, employee, customerCar, services);
        return Result.Success(order);
    }
    
    public Result<Order> ChangeAdministrator(User user)
    {
        if (!user.Roles.Contains(Role.Administrator)) return Result.Failure<Order>("User is not an employee");
        Administrator = user;
        return Result.Success(this);
    }

    public Result<Order> ChangeEmployee(User user)
    {
        if (!user.Roles.Contains(Role.Employee)) return Result.Failure<Order>("User is not an employee");
        Employee = user;
        return Result.Success(this);
    }

    public Result<Order> ChangeCustomerCar(CustomerCar customerCar)
    {
        if (CustomerCar.Customer.FullName != customerCar.Customer.FullName)
            return Result.Failure<Order>("Customer cars are not from the same customer");

        CustomerCar = customerCar;
        return Result.Success(this);
    }

    public Result<Order> ChangeStatus(Status status)
    {
        switch (status)
        {
            case Status.Completed when Status == Status.Completed:
                return Result.Failure<Order>("Order already completed");
            case Status.InProgress when Status == Status.Completed:
                return Result.Failure<Order>("Order can't be in progress after completion");
            default:
                Status = status;
                return Result.Success(this);
        }
    }

    public Result<Order> AddServices(List<Service> services)
    {
        if (services.Count == 0) return Result.Failure<Order>("The list of new services is empty");

        var duplicateServices = services.Where(s => _services.Any(os => os.Id == s.Id)).ToList();
        if (duplicateServices.Count > 0)
        {
            var duplicateServiceNames = string.Join(", ", duplicateServices.Select(s => s.Name));
            return Result.Failure<Order>($"The following services are already added: {duplicateServiceNames}");
        }

        var servicesToAdd = services.Except(_services).ToList();
        if (servicesToAdd.Count == 0) return Result.Failure<Order>("All services are already added");

        _services.AddRange(servicesToAdd);
        return Result.Success(this);
    }
}