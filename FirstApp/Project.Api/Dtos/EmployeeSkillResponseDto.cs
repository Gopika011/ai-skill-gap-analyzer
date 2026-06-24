namespace Project.Api.Dtos;

public class EmployeeSkillResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
}