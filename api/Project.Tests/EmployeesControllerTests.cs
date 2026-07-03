using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Api.Controllers;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Project.Tests;

public class EmployeesControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void GetEmployees_Should_Return_All_Employees()
    {
        // Arrange
        var context = GetDbContext();
        context.Employees.Add(new Employee { Id = "E001", Name = "Alice", Email = "alice@example.com" });
        context.Employees.Add(new Employee { Id = "E002", Name = "Bob", Email = "bob@example.com" });
        context.SaveChanges();

        var controller = new EmployeesController(context);

        // Act
        var result = controller.GetEmployees();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var employees = Assert.IsAssignableFrom<List<Employee>>(okResult.Value);
        Assert.Equal(2, employees.Count);
    }

    [Fact]
    public void CreateEmployee_Should_Create_And_Return_Created_When_Data_Is_Valid()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new EmployeesController(context);
        var dto = new CreateEmployeeDto("E001", "Alice", "alice@example.com");

        // Act
        var result = controller.CreateEmployee(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var employee = Assert.IsType<Employee>(createdResult.Value);
        Assert.Equal("E001", employee.Id);
        Assert.Equal("Alice", employee.Name);
        Assert.Equal("alice@example.com", employee.Email);
        Assert.True(context.Employees.Any(e => e.Id == "E001"));
    }

    [Fact]
    public void CreateEmployee_Should_Return_BadRequest_When_Id_Is_Missing()
    {
        // Arrange
        var context = GetDbContext();
        var controller = new EmployeesController(context);
        var dto = new CreateEmployeeDto("", "Alice", "alice@example.com");

        // Act
        var result = controller.CreateEmployee(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Employee ID is required.", badRequestResult.Value);
    }

    [Fact]
    public void ImportEmployees_Should_Import_Valid_Employees_And_Skip_Duplicates()
    {
        // Arrange
        var context = GetDbContext();
        context.Employees.Add(new Employee { Id = "E001", Name = "Alice", Email = "alice@example.com" });
        context.SaveChanges();

        var controller = new EmployeesController(context);
        var dtos = new List<CreateEmployeeDto>
        {
            new("E001", "Alice Dup", "alice@example.com"), // Skip (duplicate ID & Email)
            new("E002", "Bob", "bob@example.com"),         // Import
            new("", "Charlie", "charlie@example.com"),      // Skip (missing ID)
            new("E003", "David", "david@example.com")       // Import
        };

        // Act
        var result = controller.ImportEmployees(dtos);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        
        // Use reflection or JSON parsing since it returns an anonymous type: new { Count = importedCount }
        var value = okResult.Value;
        Assert.NotNull(value);
        
        var countProp = value.GetType().GetProperty("Count");
        Assert.NotNull(countProp);
        
        var countValue = countProp.GetValue(value);
        Assert.Equal(2, countValue); // E002 and E003 imported

        Assert.Equal(3, context.Employees.Count()); // Alice, Bob, David
        Assert.True(context.Employees.Any(e => e.Id == "E002"));
        Assert.True(context.Employees.Any(e => e.Id == "E003"));
    }
}
