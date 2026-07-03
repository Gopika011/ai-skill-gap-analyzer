namespace Project.Api.Dtos;

public class SkillResponseDto
{
    public int Id { get; set; }

    public string SkillName { get; set; } = "";

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = "";
}