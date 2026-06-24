using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

namespace Project.Api.Services;

public class SkillService : ISkillService
{
    private readonly AppDbContext _context;

    public SkillService(AppDbContext context)
    {
        _context = context;
    }

    public List<SkillResponseDto> GetSkills()
    {
        return _context.Skills
            .Include(skill => skill.Category)
            .Select(skill => new SkillResponseDto
            {
                Id = skill.Id,
                SkillName = skill.SkillName,
                Description = skill.Description,
                CategoryId = skill.CategoryId,
                CategoryName = skill.Category!.CategoryName
            })
            .ToList();
    }

    public SkillResponseDto? CreateSkill(CreateSkillDto dto)
    {
        var category = _context.Categories
            .FirstOrDefault(category => category.Id == dto.CategoryId);

        if (category is null)
        {
            return null;
        }

        Skill skill = new()
        {
            SkillName = dto.SkillName,
            Description = dto.Description,
            CategoryId = dto.CategoryId
        };

        _context.Skills.Add(skill);

        _context.SaveChanges();

        return new SkillResponseDto
        {
            Id = skill.Id,
            SkillName = skill.SkillName,
            Description = skill.Description,
            CategoryId = skill.CategoryId,
            CategoryName = category.CategoryName
        };
    }

    public bool UpdateSkill(int skillId, CreateSkillDto dto)
    {
        var skill = _context.Skills
            .FirstOrDefault(skill => skill.Id == skillId);

        if (skill is null)
        {
            return false;
        }

        skill.SkillName = dto.SkillName;
        skill.Description = dto.Description;
        skill.CategoryId = dto.CategoryId;

        _context.SaveChanges();

        return true;
    }

    public bool DeleteSkill(int skillId)
    {
        var skill = _context.Skills
            .FirstOrDefault(skill => skill.Id == skillId);

        if (skill is null)
        {
            return false;
        }

        _context.Skills.Remove(skill);

        _context.SaveChanges();

        return true;
    }
}