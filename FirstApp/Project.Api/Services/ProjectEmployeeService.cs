using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

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
            .Include(pe => pe.User)
            .Select(pe => new EmployeeDto
            {
                Id = pe.User.Id,
                Name = pe.User.Email
            })
            .ToList();
    }
    public bool AssignEmployee(int projectId, AssignEmployeeDto dto)
    {
        var project = _context.Projects.Find(projectId);

        var user = _context.Users.Find(dto.UserId);

        if (project is null || user is null)
        {
            return false;
        }

        var exists = _context.ProjectEmployees.Any(pe =>
            pe.ProjectId == projectId &&
            pe.UserId == dto.UserId);

        if (exists)
        {
            return false;
        }

        var assignment = new ProjectEmployee
        {
            ProjectId = projectId,
            UserId = dto.UserId
        };

        _context.ProjectEmployees.Add(assignment);

        _context.SaveChanges();

        return true;
    }

    public bool RemoveEmployee(int projectId, int userId)
    {
        var assignment = _context.ProjectEmployees
            .FirstOrDefault(pe =>
                pe.ProjectId == projectId &&
                pe.UserId == userId);

        if (assignment is null)
        {
            return false;
        }

        _context.ProjectEmployees.Remove(assignment);

        _context.SaveChanges();

        return true;
    }
}