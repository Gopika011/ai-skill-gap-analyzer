using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

namespace Project.Api.Services;

public class EmployeeSkillService : IEmployeeSkillService
{
    private readonly AppDbContext _context;

    public EmployeeSkillService(AppDbContext context)
    {
        _context = context;
    }

    public List<EmployeeSkillResponseDto> GetEmployeeSkills(int userId)
    {
        return _context.EmployeeSkill
            .Where(es => es.UserId == userId)
            .Include(es => es.Skill)
            .ThenInclude(s => s!.Category)
            .Select(es => new EmployeeSkillResponseDto
            {
                Id = es.Id,
                UserId = es.UserId,
                SkillId = es.SkillId,
                SkillName = es.Skill!.SkillName,
                CategoryName = es.Skill.Category!.CategoryName,
                ProficiencyLevel = es.ProficiencyLevel,
                YearsOfExperience = es.YearsOfExperience
            })
            .ToList();
    }

    public EmployeeSkillResponseDto? AddSkill(int userId, CreateEmployeeSkillDto dto)
    {
        // Check user exists
        var userExists = _context.Users.Any(u => u.Id == userId);
        if (!userExists) return null;

        // Check skill exists
        var skill = _context.Skills
            .Include(s => s.Category)
            .FirstOrDefault(s => s.Id == dto.SkillId);
        if (skill is null) return null;

        // Prevent duplicate — same user can't add same skill twice
        var alreadyExists = _context.EmployeeSkill
            .Any(es => es.UserId == userId && es.SkillId == dto.SkillId);
        if (alreadyExists) return null;

        var employeeSkill = new EmployeeSkill
        {
            UserId = userId,
            SkillId = dto.SkillId,
            ProficiencyLevel = dto.ProficiencyLevel,
            YearsOfExperience = dto.YearsOfExperience
        };

        _context.EmployeeSkill.Add(employeeSkill);
        _context.SaveChanges();

        return new EmployeeSkillResponseDto
        {
            Id = employeeSkill.Id,
            UserId = employeeSkill.UserId,
            SkillId = employeeSkill.SkillId,
            SkillName = skill.SkillName,
            CategoryName = skill.Category!.CategoryName,
            ProficiencyLevel = employeeSkill.ProficiencyLevel,
            YearsOfExperience = employeeSkill.YearsOfExperience
        };
    }

    public bool DeleteSkill(int userId, int skillId)
    {
        var employeeSkill = _context.EmployeeSkill
            .FirstOrDefault(es => es.UserId == userId && es.SkillId == skillId);

        if (employeeSkill is null) return false;

        _context.EmployeeSkill.Remove(employeeSkill);
        _context.SaveChanges();

        return true;
    }
}