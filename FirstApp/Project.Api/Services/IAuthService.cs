using Project.Api.Dtos;

namespace Project.Api.Services;

public interface IAuthService
{
    UserResponse? Register(CreateUserDto dto);

    UserResponse? Login(CreateUserDto dto);
}