namespace Project.Api.Entities;

public class ProjectEmployee
{
    public int ProjectId { get; set; }
    public Projectt Project { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}