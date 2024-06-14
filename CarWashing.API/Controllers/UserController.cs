using CarWashing.Application.Services;
using CarWashing.Contracts.User;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CarWashing.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator))]
public class UserController(UserService userService) : ControllerBase
{
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers([FromQuery] UserFilter filter)
    {
        var users = await userService.GetUsers(filter);
        if (users.IsFailure) return BadRequest(users.Error);
        var userResponses = users.Value
            .Select(user => new UserResponse(user.Id, user.FirstName, user.LastName, user.Patronymic, user.Email,
                user.IsSendNotify, user.Roles.ToList()));
        return Ok(userResponses);
    }

    // GET: api/User/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        var user = await userService.GetUser(id);

        if (user == null)
        {
            return NotFound();
        }

        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Patronymic, user.Email,
            user.IsSendNotify, user.Roles.ToList());
    }

    // PUT: api/User/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutUser(int id, UserRequest request)
    {
        var result = await userService.UpdateUser(id, request.FirstName, request.LastName, request.Patronymic,
            request.Email, request.IsSendNotify);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/Register
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest request)
    {
        var result = await userService.Register(request.FirstName, request.LastName, request.Patronymic, request.Email,
            request.Password, request.IsSendNotify);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    // POST: api/Login
    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Login(LoginUserRequest userRequest)
    {
        var result = await userService.Login(userRequest.Email, userRequest.Password);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    // PUT: api/AddRoles
    [HttpPut("ChangeRoles")]
    public async Task<ActionResult<UserResponse>> ChangeRoles(int id, [FromBody] List<Role> roles)
    {
        await userService.ChangeRoles(id, roles);
        return Ok();
    }

    // DELETE: api/User/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await userService.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }

        await userService.DeleteUser(id);

        return Ok("Deleted");
    }
}