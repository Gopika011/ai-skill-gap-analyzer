namespace Project.Api.Dtos;

public class EmployeeDto
{
    public required string Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}