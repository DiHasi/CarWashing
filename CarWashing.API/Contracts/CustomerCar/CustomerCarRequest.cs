namespace CarWashing.Contracts.CustomerCar;

public record CustomerCarRequest(
    int CarId,
    int CustomerId,
    int Year,
    string Number
    );