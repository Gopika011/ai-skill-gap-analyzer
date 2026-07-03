using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register(CreateUserDto dto)
    {
        var result = _authService.Register(dto);

        if(result is null)
        {
            return BadRequest("User already exists");
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public IActionResult Login(CreateUserDto dto)
    {
        var result = _authService.Login(dto);

        if(result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}