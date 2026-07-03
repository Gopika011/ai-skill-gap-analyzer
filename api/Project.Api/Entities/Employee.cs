using System.Collections.Generic;

namespace Project.Api.Entities;

public class Employee
{
    public required string Id { get; set; } // Primary Key (e.g., "E001")

    public required string Name { get; set; }

    public required string Email { get; set; }

    // Navigation properties
    public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        = new List<ProjectEmployee>();

    public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        = new List<EmployeeSkill>();
}
