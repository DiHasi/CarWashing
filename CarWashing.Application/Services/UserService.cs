using CarWashing.Application.Interfaces.Auth;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;
using CSharpFunctionalExtensions;
using Role = CarWashing.Domain.Enums.Role;

namespace CarWashing.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
{
    public async Task<Result<IEnumerable<User>>> GetUsers(UserFilter filter)
    {
        var users = await userRepository.GetUsers(filter);
        
        return Result.Success(users);
    }

    public async Task<User?> GetUser(int id)
    {
        return await userRepository.GetUser(id);
    }

    public async Task<Result<User>> UpdateUser(int id, string firstName, string lastName, string? patronymic,
        string email, bool isSendNotify)
    {
        var userToUpdate = await GetUser(id);
        if (userToUpdate == null) return Result.Failure<User>("User not found");

        var result = userToUpdate.ChangeFirstName(firstName);
        if (result.IsFailure) return Result.Failure<User>(result.Error);

        result = result.Value.ChangeLastName(lastName);
        if (result.IsFailure) return Result.Failure<User>(result.Error);

        result = result.Value.ChangePatronymic(patronymic);
        if (result.IsFailure) return Result.Failure<User>(result.Error);

        result = result.Value.ChangeEmail(email);
        if (result.IsFailure) return Result.Failure<User>(result.Error);

        result = result.Value.ChangeIsSendNotify(isSendNotify);
        if (result.IsFailure) return Result.Failure<User>(result.Error);

        await userRepository.UpdateUser(result.Value);

        return Result.Success(result.Value);
    }

    public async Task DeleteUser(int id)
    {
        await userRepository.DeleteUser(id);
    }

    public async Task<Result<string>> Register(string firstName, string lastName, string? patronymic, string email,
        string password, bool isSendNotify)
    {
        if (await userRepository.GetUserByEmail(email) != null)
        {
            return Result.Failure<string>("User with this email already exists");
        }

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
        {
            return Result.Failure<string>("Data cannot be empty");
        }

        if (password.Length < 8)
        {
            return Result.Failure<string>("Password must be at least 8 characters long");
        }

        var role = await userRepository.GetRole(Role.User);
        var hash = passwordHasher.Generate(password);
        
        
        
        var result = User.Create(firstName, lastName, patronymic, email, hash, isSendNotify, [Role.User]);
        
        if(result.IsFailure) return Result.Failure<string>(result.Error);
        
        await userRepository.AddUser(result.Value);

        var token = jwtProvider.GenerateToken(result.Value);
        return Result.Success(token);
    }

    public async Task<Result<string>> Login(string email, string password)
    {
        var user = await userRepository.GetUserByEmail(email);
        if (user == null) return Result.Failure<string>("User not found");

        if (!passwordHasher.Verify(password, user.PasswordHash)) return Result.Failure<string>("Wrong password");

        var token = jwtProvider.GenerateToken(user);
        return Result.Success(token);
    }

    public async Task<Result> ChangeRoles(int userId, List<Role> roles)
    {
        var user = await userRepository.GetUser(userId);
        if (user == null) return Result.Failure<Result>("User not found");

        await userRepository.ChangeUserRoles(userId, roles);
        return Result.Success();
    }
}