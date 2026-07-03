using Microsoft.AspNetCore.Mvc;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

namespace Project.Api.Controllers;

[ApiController]
[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        return Ok(_context.Categories.ToList());
    }

    [HttpPost]
    public IActionResult CreateCategory(CreateCategoryDto dto)
    {
        var category = new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        _context.SaveChanges();
        return Ok(category);
    }
}