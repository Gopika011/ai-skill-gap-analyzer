namespace Project.Api.Dtos;

public class CreateEmployeeSkillDto
{
    public int SkillId { get; set; }
    public required string ProficiencyLevel { get; set; }
    public int YearsOfExperience { get; set; }
}