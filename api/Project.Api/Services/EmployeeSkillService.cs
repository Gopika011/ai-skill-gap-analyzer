using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Project.Api.Services;

public class EmployeeSkillService : IEmployeeSkillService
{
    private readonly AppDbContext _context;

    public EmployeeSkillService(AppDbContext context)
    {
        _context = context;
    }

    public List<EmployeeSkillResponseDto> GetEmployeeSkills(string employeeId)
    {
        return _context.EmployeeSkill
            .Where(es => es.EmployeeId == employeeId)
            .Include(es => es.Skill)
            .ThenInclude(s => s!.Category)
            .Select(es => new EmployeeSkillResponseDto
            {
                Id = es.Id,
                EmployeeId = es.EmployeeId,
                SkillId = es.SkillId,
                SkillName = es.Skill!.SkillName,
                CategoryName = es.Skill.Category!.CategoryName,
                ProficiencyLevel = es.ProficiencyLevel,
                YearsOfExperience = es.YearsOfExperience
            })
            .ToList();
    }

    public EmployeeSkillResponseDto? AddSkill(string employeeId, CreateEmployeeSkillDto dto)
    {
        // Check employee exists
        var employeeExists = _context.Employees.Any(e => e.Id == employeeId);
        if (!employeeExists) return null;

        // Check skill exists
        var skill = _context.Skills
            .Include(s => s.Category)
            .FirstOrDefault(s => s.Id == dto.SkillId);
        if (skill is null) return null;

        // Prevent duplicate — same employee can't add same skill twice
        var alreadyExists = _context.EmployeeSkill
            .Any(es => es.EmployeeId == employeeId && es.SkillId == dto.SkillId);
        if (alreadyExists) return null;

        var employeeSkill = new EmployeeSkill
        {
            EmployeeId = employeeId,
            SkillId = dto.SkillId,
            ProficiencyLevel = dto.ProficiencyLevel,
            YearsOfExperience = dto.YearsOfExperience
        };

        _context.EmployeeSkill.Add(employeeSkill);
        _context.SaveChanges();

        return new EmployeeSkillResponseDto
        {
            Id = employeeSkill.Id,
            EmployeeId = employeeSkill.EmployeeId,
            SkillId = employeeSkill.SkillId,
            SkillName = skill.SkillName,
            CategoryName = skill.Category!.CategoryName,
            ProficiencyLevel = employeeSkill.ProficiencyLevel,
            YearsOfExperience = employeeSkill.YearsOfExperience
        };
    }

    public bool DeleteSkill(string employeeId, int skillId)
    {
        var employeeSkill = _context.EmployeeSkill
            .FirstOrDefault(es => es.EmployeeId == employeeId && es.SkillId == skillId);

        if (employeeSkill is null) return false;

        _context.EmployeeSkill.Remove(employeeSkill);
        _context.SaveChanges();

        return true;
    }
}