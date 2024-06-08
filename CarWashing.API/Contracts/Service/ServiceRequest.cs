namespace CarWashing.Contracts.Service;

public record ServiceRequest(
    string Name,
    int Price,
    int Time
    );