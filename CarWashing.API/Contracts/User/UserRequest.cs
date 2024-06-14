namespace CarWashing.Contracts.User;

public record UserRequest(
    string FirstName,
    string LastName,
    string Patronymic,
    string Email,
    bool IsSendNotify
    );