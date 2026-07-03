namespace Project.Api.Entities;

public class Skill
{
    public int Id { get; set; }

    public required string SkillName { get; set; }

    public string? Description { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }

    // Navigation Property
    public Category? Category { get; set; }

    public ICollection<ProjectRequirement> ProjectRequirements { get; set; }
        = new List<ProjectRequirement>();
}