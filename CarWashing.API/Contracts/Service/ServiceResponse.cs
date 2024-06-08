using CarWashing.Domain.ValueObjects;

namespace CarWashing.Contracts.Service;

public record ServiceResponse(
    int Id,
    string Name,
    Price Price,
    Time Time
);
