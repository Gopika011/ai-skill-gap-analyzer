using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Services;
using Project.Api.Entities;
using System;
using Xunit;

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
        var dto = new CreateUserDto("test@gmail.com", "1234");

        // Act
        var result = service.Register(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@gmail.com", result.email);
        Assert.True(context.Users.Any(u => u.Email == "test@gmail.com"));
    }

    [Fact]
    public void Register_Should_Return_Null_When_Email_Is_Already_Registered()
    {
        // Arrange
        var context = GetDbContext();
        context.Users.Add(new User { Email = "existing@gmail.com", Password = "password" });
        context.SaveChanges();

        var service = new AuthService(context);
        var dto = new CreateUserDto("existing@gmail.com", "newpassword");

        // Act
        var result = service.Register(dto);

        // Assert
        Assert.Null(result);
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
        var dto = new CreateUserDto("test@gmail.com", "1234");

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@gmail.com", result.email);
    }

    [Fact]
    public void Login_Should_Return_Null_When_Email_Does_Not_Exist()
    {
        // Arrange
        var context = GetDbContext();
        var service = new AuthService(context);
        var dto = new CreateUserDto("nonexistent@gmail.com", "password");

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Login_Should_Return_Null_When_Password_Is_Incorrect()
    {
        // Arrange
        var context = GetDbContext();
        context.Users.Add(new User { Email = "user@gmail.com", Password = "correctpassword" });
        context.SaveChanges();

        var service = new AuthService(context);
        var dto = new CreateUserDto("user@gmail.com", "wrongpassword");

        // Act
        var result = service.Login(dto);

        // Assert
        Assert.Null(result);
    }
}