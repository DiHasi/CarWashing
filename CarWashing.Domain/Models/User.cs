using System.Text.RegularExpressions;
using CarWashing.Domain.Enums;
using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public partial class User
{
    private User(string firstName, string lastName, string? patronymic, string email, string passwordHash,
        bool isSendNotify, List<Role> roles)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
        Email = email;
        PasswordHash = passwordHash;
        IsSendNotify = isSendNotify;
        _roles = roles;
    }

    private const string NameRegexString = "^[a-zA-Zа-яА-Я]+$";
    private const string EmailRegexString = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";

    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Patronymic { get; private set; }
    public string FullName => $"{FirstName} {LastName} {Patronymic}".Trim();
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsSendNotify { get; private set; }
    private List<Role>? _roles = [];
    public IReadOnlyList<Role> Roles => _roles ??= [];


    public static Result<User> Create(string firstName, string lastName, string? patronymic, string email,
        string passwordHash, bool isSendNotify, List<Role> roles)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
        {
            return Result.Failure<User>("Data cannot be empty");
        }

        return Result.Success(new User(firstName, lastName, patronymic, email, passwordHash, isSendNotify, roles));
    }

    public Result<User> AddRole(Role role)
    {
        _roles?.Add(role);
        return Result.Success(this);
    }

    public Result<User> ChangeRoles(List<Role> roles)
    {
        _roles?.Clear();

        _roles?.AddRange(roles);

        return Result.Success(this);
    }

    public Result<User> RemoveRole(Role role)
    {
        _roles?.Remove(role);
        return Result.Success(this);
    }

    public Result<User> ChangeIsSendNotify(bool isSendNotify)
    {
        IsSendNotify = isSendNotify;
        return Result.Success(this);
    }

    public Result<User> ChangeEmail(string email)
    {
        if (!EmailRegex().IsMatch(email))
        {
            return Result.Failure<User>("Email is not valid");
        }

        Email = email;
        return Result.Success(this);
    }

    public Result<User> ChangePatronymic(string? patronymic)
    {
        if (!string.IsNullOrEmpty(patronymic))
        {
            if (!NameRegex().IsMatch(patronymic))
            {
                return Result.Failure<User>("Patronymic is not valid");
            }
        }

        Patronymic = patronymic;
        return Result.Success(this);
    }

    public Result<User> ChangeFirstName(string firstName)
    {
        if (!NameRegex().IsMatch(firstName))
        {
            return Result.Failure<User>("Name is not valid");
        }

        FirstName = firstName;
        return Result.Success(this);
    }

    public Result<User> ChangeLastName(string lastName)
    {
        if (!NameRegex().IsMatch(lastName))
        {
            return Result.Failure<User>("Name is not valid");
        }

        LastName = lastName;
        return Result.Success(this);
    }

    [GeneratedRegex(EmailRegexString)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(NameRegexString)]
    private static partial Regex NameRegex();
}