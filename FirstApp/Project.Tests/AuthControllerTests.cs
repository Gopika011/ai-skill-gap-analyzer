using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.Api.Controllers;
using Project.Api.Dtos;
using Project.Api.Services;

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

        Assert.IsType<OkObjectResult>(result);
    }
}