using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

namespace Project.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public UserResponse? Register(CreateUserDto dto)
    {
        var exist = _context.Users
            .FirstOrDefault(user => user.Email == dto.email);

        if (exist is not null)
        {
            return null;
        }

        User user = new()
        {
            Email = dto.email,
            Password = dto.pass
        };

        _context.Users.Add(user);

        _context.SaveChanges();

        return new UserResponse(user.Id, user.Email);
    }

    public UserResponse? Login(CreateUserDto dto)
    {
        var user = _context.Users.FirstOrDefault(user =>
            user.Email == dto.email &&
            user.Password == dto.pass
        );

        if (user is null)
        {
            return null;
        }

        return new UserResponse(user.Id, user.Email);
    }
}