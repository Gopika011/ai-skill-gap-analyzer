namespace Project.Api.Entities;

public class ProjectEmployee
{
    public int ProjectId { get; set; }
    public Projectt Project { get; set; } = null!;

    public required string EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}