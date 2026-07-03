using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Api.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetEmployees()
    {
        var employees = _context.Employees
            .OrderBy(e => e.Id)
            .ToList();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployee(string id)
    {
        var employee = _context.Employees.Find(id);
        if (employee is null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployee(CreateEmployeeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            return BadRequest("Employee ID is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Name is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        string employeeId = dto.Id.Trim().ToUpper();
        if (_context.Employees.Any(e => e.Id == employeeId))
        {
            return BadRequest($"Employee with ID '{employeeId}' already exists.");
        }

        // Check unique email
        if (_context.Employees.Any(e => e.Email.ToLower() == dto.Email.ToLower()))
        {
            return BadRequest($"An employee with email '{dto.Email}' already exists.");
        }

        var employee = new Employee
        {
            Id = employeeId,
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim().ToLower()
        };

        _context.Employees.Add(employee);
        _context.SaveChanges();

        return Created($"/employees/{employee.Id}", employee);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(string id, CreateEmployeeDto dto)
    {
        var employee = _context.Employees.Find(id);
        if (employee is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Name is required");
        }
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required");
        }

        // Check email uniqueness if email is changing
        if (employee.Email.ToLower() != dto.Email.ToLower() &&
            _context.Employees.Any(e => e.Email.ToLower() == dto.Email.ToLower()))
        {
            return BadRequest($"An employee with email '{dto.Email}' already exists.");
        }

        employee.Name = dto.Name.Trim();
        employee.Email = dto.Email.Trim().ToLower();

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(string id)
    {
        var employee = _context.Employees
            .Include(e => e.EmployeeSkills)
            .Include(e => e.ProjectEmployees)
            .FirstOrDefault(e => e.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        // Delete associated records to avoid constraint violations
        _context.EmployeeSkill.RemoveRange(employee.EmployeeSkills);
        _context.ProjectEmployees.RemoveRange(employee.ProjectEmployees);
        _context.Employees.Remove(employee);
        
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost("import")]
    public IActionResult ImportEmployees(List<CreateEmployeeDto> dtos)
    {
        if (dtos == null || dtos.Count == 0)
        {
            return BadRequest("No employees provided for import.");
        }

        var importedCount = 0;
        var existingEmails = _context.Employees.Select(e => e.Email.ToLower()).ToHashSet();
        var existingIds = _context.Employees.Select(e => e.Id.ToUpper()).ToHashSet();

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Id) || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
                {
                    continue; // Skip invalid records (all fields are required)
                }

                var emailNormalized = dto.Email.Trim().ToLower();
                if (existingEmails.Contains(emailNormalized))
                {
                    continue; // Skip duplicate email
                }

                string employeeId = dto.Id.Trim().ToUpper();
                if (existingIds.Contains(employeeId))
                {
                    continue; // Skip duplicate ID
                }

                var employee = new Employee
                {
                    Id = employeeId,
                    Name = dto.Name.Trim(),
                    Email = emailNormalized
                };

                _context.Employees.Add(employee);
                existingEmails.Add(emailNormalized);
                existingIds.Add(employeeId);
                importedCount++;
            }

            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, $"An error occurred during import: {ex.Message}");
        }

        return Ok(new { Count = importedCount });
    }
}
