using CarWashing.Contracts.Car;

namespace CarWashing.Contracts.CustomerCar;

public record CustomerCarResponse(
    int Id,
    CarResponse Car,
    string CustomerFullName,
    string CustomerEmail,
    int Year,
    string Number
    );