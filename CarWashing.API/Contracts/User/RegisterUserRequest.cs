namespace CarWashing.Contracts.User;

public record RegisterUserRequest(
    string FirstName,
    string LastName,
    string? Patronymic,
    string Email,
    string Password,
    bool IsSendNotify
    );