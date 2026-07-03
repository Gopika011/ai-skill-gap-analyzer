using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.Api.Controllers;
using Project.Api.Dtos;
using Project.Api.Services;
using Xunit;

namespace Project.Tests;

public class AuthControllerTests
{
    [Fact]
    public void Login_Should_Return_Ok_When_Credentials_Are_Correct()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(service => service.Login(It.IsAny<CreateUserDto>()))
            .Returns(new UserResponse(1, "test@gmail.com"));

        var controller = new AuthController(mockAuthService.Object);
        var dto = new CreateUserDto("test@gmail.com", "1234");

        // Act
        var result = controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal("test@gmail.com", response.email);
    }

    [Fact]
    public void Login_Should_Return_Unauthorized_When_Credentials_Are_Incorrect()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(service => service.Login(It.IsAny<CreateUserDto>()))
            .Returns((UserResponse?)null);

        var controller = new AuthController(mockAuthService.Object);
        var dto = new CreateUserDto("test@gmail.com", "wrongpass");

        // Act
        var result = controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void Register_Should_Return_Ok_When_Registration_Succeeds()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(service => service.Register(It.IsAny<CreateUserDto>()))
            .Returns(new UserResponse(1, "newuser@gmail.com"));

        var controller = new AuthController(mockAuthService.Object);
        var dto = new CreateUserDto("newuser@gmail.com", "password123");

        // Act
        var result = controller.Register(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal("newuser@gmail.com", response.email);
    }

    [Fact]
    public void Register_Should_Return_BadRequest_When_User_Already_Exists()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(service => service.Register(It.IsAny<CreateUserDto>()))
            .Returns((UserResponse?)null);

        var controller = new AuthController(mockAuthService.Object);
        var dto = new CreateUserDto("existing@gmail.com", "password");

        // Act
        var result = controller.Register(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User already exists", badRequestResult.Value);
    }
}