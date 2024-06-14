using CarWashing.Domain.Enums;

namespace CarWashing.Contracts.User;

public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string? Patronymic,
    string Email,
    bool IsSendNotify,
    List<Role> Roles);