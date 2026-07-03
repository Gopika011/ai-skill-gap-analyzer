namespace Project.Api.Entities;

public class Projectt
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        = new List<ProjectEmployee>();

    public ICollection<ProjectRequirement> Requirements { get; set; }
        = new List<ProjectRequirement>();
}