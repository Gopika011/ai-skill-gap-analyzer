namespace Project.Api.Dtos;

public class CreateCategoryDto
{
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
}