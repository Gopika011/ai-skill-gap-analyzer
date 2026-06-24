using Microsoft.AspNetCore.Mvc;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    // Context injected by ASP.NET
    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _context.Users.ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserDto newUser)
    {
        if (string.IsNullOrEmpty(newUser.email))
        {
            return BadRequest("Email is required");
        }

        User user = new()
        {
            Email = newUser.email,
            Password = newUser.pass
        };

        _context.Users.Add(user);

        _context.SaveChanges();

        return Created(
            $"/users/{user.Id}",
            new UserResponse(user.Id, user.Email)
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, CreateUserDto updUser)
    {
        var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        user.Email = updUser.email;
        user.Password = updUser.pass;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users
            .FirstOrDefault(user => user.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);

        _context.SaveChanges();

        return NoContent();
    }
}