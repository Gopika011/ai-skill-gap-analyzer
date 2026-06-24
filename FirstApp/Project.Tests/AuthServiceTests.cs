using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Services;
using Project.Api.Entities;

namespace Project.Tests;
public class AuthServiceTests
{

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }


    [Fact]
    public void Register_Should_Create_User()
    {
        // Arrange
        var context = GetDbContext();

        var service = new AuthService(context);

        var dto = new CreateUserDto
        (
            "test@gmail.com",
            "1234"
        );

        // Act

        var result = service.Register(dto);

        // Assert

        Assert.NotNull(result);

        Assert.Equal("test@gmail.com", result.email);
    }

    [Fact]
    public void Login_Should_Return_User_When_Credentials_Are_Correct()
    {
        // Arrange

        var context = GetDbContext();

        context.Users.Add(new User
        {
            Email = "test@gmail.com",
            Password = "1234"
        });

        context.SaveChanges();

        var service = new AuthService(context);

        var dto = new CreateUserDto
        (
            "test@gmail.com",
            "1234"
        );

        // Act

        var result = service.Login(dto);

        // Assert

        Assert.NotNull(result);

        Assert.Equal("test@gmail.com", result.email);
    }
}