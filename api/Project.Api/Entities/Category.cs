namespace Project.Api.Entities;

public class Category
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public string? Description { get; set; }

    public List<Skill> Skills { get; set; } = [];
}