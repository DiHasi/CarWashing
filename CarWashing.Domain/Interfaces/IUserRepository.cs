using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;


namespace CarWashing.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers(UserFilter filter);
    Task<User?> GetUser(int id);
    Task<User?> GetUserByEmail(string email);
    Task<User> AddUser(User user);
    Task<Role> GetRole(Role role);
    Task UpdateUser(User user);
    Task DeleteUser(int id);
    Task ChangeUserRoles(int id, List<Role> roles);
}