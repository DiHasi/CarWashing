using System.Collections;
using CSharpFunctionalExtensions;

namespace CarWashing.Domain.Models;

public class User
{
    private User(string firstName, string lastName, string? patronymic, string email, bool isSendNotify)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
        Email = email;
        IsSendNotify = isSendNotify;
    }

    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Patronymic { get; private set; }
    public string Email { get; private set; }
    public bool IsSendNotify { get; private set; }
    private List<Role>? _roles { get; set; }
    public IReadOnlyList<Role> Roles => _roles ??= [];
    
    public static Result<User> Create(string firstName, string lastName, string? patronymic, string email, bool isSendNotify)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
        {
            return Result.Failure<User>("Data must be not empty");
        }
        
        return Result.Success(new User(firstName, lastName, patronymic, email, isSendNotify));
    }
    
    public void AddRole(Role role)
    {
        _roles?.Add(role);
    }
    
    public void RemoveRole(Role role)
    {
        _roles?.Remove(role);
    }
    
    public void ChangeIsSendNotify(bool isSendNotify)
    {
        IsSendNotify = isSendNotify;
    }
    
    public void ChangeEmail(string email)
    {
        Email = email;
    }
    
    public void ChangePatronymic(string? patronymic)
    {
        Patronymic = patronymic;
    }
    
    public void ChangeFirstName(string firstName)
    {
        FirstName = firstName;
    }
    
    public void ChangeLastName(string lastName)
    {
        LastName = lastName;
    }
}