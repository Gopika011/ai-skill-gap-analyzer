using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;
using Project.Api.Services;
using System;
using System.Linq;
using Xunit;

namespace Project.Tests;

public class ProjectServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void GetProjects_Should_Return_List_Of_Projects()
    {
        // Arrange
        var context = GetDbContext();
        context.Projects.Add(new Projectt { Name = "Alpha", Description = "A project", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) });
        context.Projects.Add(new Projectt { Name = "Beta", Description = "B project", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) });
        context.SaveChanges();

        var service = new ProjectService(context);

        // Act
        var result = service.GetProjects();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Alpha");
        Assert.Contains(result, p => p.Name == "Beta");
    }

    [Fact]
    public void CreateProject_Should_Add_Project_To_Database()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);
        var dto = new CreateProjectDto
        {
            Name = "Project Gamma",
            Description = "Gamma description",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10)
        };

        // Act
        var result = service.CreateProject(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Project Gamma", result.Name);
        Assert.Equal("Gamma description", result.Description);
        Assert.True(context.Projects.Any(p => p.Name == "Project Gamma"));
    }

    [Fact]
    public void DeleteProject_Should_Remove_Project_From_Database_When_Exists()
    {
        // Arrange
        var context = GetDbContext();
        var project = new Projectt { Name = "Delete Me", Description = "Test delete", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) };
        context.Projects.Add(project);
        context.SaveChanges();

        var service = new ProjectService(context);

        // Act
        var result = service.DeleteProject(project.Id);

        // Assert
        Assert.True(result);
        Assert.False(context.Projects.Any(p => p.Id == project.Id));
    }

    [Fact]
    public void DeleteProject_Should_Return_False_When_Project_Does_Not_Exist()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);

        // Act
        var result = service.DeleteProject(999);

        // Assert
        Assert.False(result);
    }
}
