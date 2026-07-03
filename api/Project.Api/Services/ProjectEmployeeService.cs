using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Project.Api.Services;

public class ProjectEmployeeService : IProjectEmployeeService
{
    private readonly AppDbContext _context;

    public ProjectEmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public List<EmployeeDto> GetEmployees(int projectId)
    {
        return _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .Include(pe => pe.Employee)
            .Select(pe => new EmployeeDto
            {
                Id = pe.Employee.Id,
                Name = pe.Employee.Name,
                Email = pe.Employee.Email
            })
            .ToList();
    }

    public bool AssignEmployee(int projectId, AssignEmployeeDto dto)
    {
        var project = _context.Projects.Find(projectId);
        var employee = _context.Employees.Find(dto.EmployeeId);

        if (project is null || employee is null)
        {
            return false;
        }

        var exists = _context.ProjectEmployees.Any(pe =>
            pe.ProjectId == projectId &&
            pe.EmployeeId == dto.EmployeeId);

        if (exists)
        {
            return false;
        }

        var assignment = new ProjectEmployee
        {
            ProjectId = projectId,
            EmployeeId = dto.EmployeeId
        };

        _context.ProjectEmployees.Add(assignment);
        _context.SaveChanges();

        return true;
    }

    public bool RemoveEmployee(int projectId, string employeeId)
    {
        var assignment = _context.ProjectEmployees
            .FirstOrDefault(pe =>
                pe.ProjectId == projectId &&
                pe.EmployeeId == employeeId);

        if (assignment is null)
        {
            return false;
        }

        _context.ProjectEmployees.Remove(assignment);
        _context.SaveChanges();

        return true;
    }
}