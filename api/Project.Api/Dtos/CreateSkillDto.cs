namespace Project.Api.Dtos;

public class CreateSkillDto
{
    public required string SkillName { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }
}