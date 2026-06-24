namespace Project.Api.Entities;

public class ProjectRequirement
{
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public Projectt Project { get; set; } = null!;

    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public string RequiredLevel { get; set; } = string.Empty;
}