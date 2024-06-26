using System.Linq.Expressions;
using AutoFilter;
using AutoMapper;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Context;
using CarWashing.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Role = CarWashing.Domain.Enums.Role;

namespace CarWashing.Persistence.Repositories;

public class UserRepository(CarWashingContext context, IMapper mapper) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsers(UserFilter filter)
    {
        var query = context.Users
            .Include(u => u.Roles)
            .AsNoTracking();

        query = filter.ByDescending ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id);
        query = query.AutoFilter(filter);

        if (filter.Roles != null && filter.Roles.Count != 0)
        {
            var userIdsWithRoles = context.Roles
                .Include(r => r.Users)
                .Where(ur => filter.Roles.Contains((Role)ur.Id))
                .SelectMany(r => r.Users!.Select(u => u.Id))
                .Distinct();

            query = query.Where(u => userIdsWithRoles.Contains(u.Id));
        }

        if (filter.OrderBy.HasValue)
        {
            var sortBy = filter.OrderBy.Value.GetPath();
            var parameter = Expression.Parameter(typeof(UserEntity), "o");
            Expression property;
            if (sortBy.Contains('.'))
            {
                var parts = sortBy.Split('.');
                property = Expression.Property(parameter, parts[0]);
                for (var i = 1; i < parts.Length; i++)
                {
                    property = Expression.Property(property, parts[i]);
                }
            }
            else
            {
                property = Expression.Property(parameter, sortBy);
            }

            var converted = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<UserEntity, object>>(converted, parameter);

            query = filter.ByDescending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }

        query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

        var userEntities = await query.ToListAsync();

        return mapper.Map<IEnumerable<User>>(userEntities);
    }

    public async Task<User?> GetUser(int id)
    {
        var userEntity = await context.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return mapper.Map<User>(userEntity);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var userEntity = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email) ?? null;

        return mapper.Map<User>(userEntity);
    }

    public async Task<User> AddUser(User user)
    {
        var userEntity = mapper.Map<UserEntity>(user);

        context.Entry(userEntity).State = EntityState.Unchanged;
        
        var addedUser = context.Users.Add(userEntity).Entity;
        
        if (addedUser.Roles != null)
            foreach (var roleEntity in addedUser.Roles)
            {
                context.Entry(roleEntity).State = EntityState.Modified;
            }
        
        await context.SaveChangesAsync();

        return mapper.Map<User>(addedUser);
    }

    public async Task<Role> GetRole(Role role)
    {
        var roleEntity = await context
            .Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == role.ToString());
        return mapper.Map<Role>(roleEntity);
    }

    public async Task UpdateUser(User service)
    {
        var serviceEntity = await context.Users.FindAsync(service.Id);
        if (serviceEntity != null)
        {
            serviceEntity.FirstName = service.FirstName;
            serviceEntity.LastName = service.LastName;
            serviceEntity.Patronymic = service.Patronymic;
            serviceEntity.Email = service.Email;
            serviceEntity.PasswordHash = service.PasswordHash;
            serviceEntity.IsSendNotify = service.IsSendNotify;

            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null) context.Users.Remove(user);

        await context.SaveChangesAsync();
    }

    public async Task ChangeUserRoles(int id, List<Role> roles)
    {
        var userEntity = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity == null) return;

        var currentRoles = userEntity.Roles;

        currentRoles?.Clear();

        foreach (var role in roles.Where(role => Enum.IsDefined(typeof(Role), role)))
        {
            var roleEntity = await context.Roles.FindAsync((int)role);

            if (roleEntity != null) currentRoles?.Add(roleEntity);
        }

        await context.SaveChangesAsync();
    }
}