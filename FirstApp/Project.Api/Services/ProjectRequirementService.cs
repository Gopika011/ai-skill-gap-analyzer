using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

public class ProjectRequirementService
    : IProjectRequirementService
{
    private readonly AppDbContext _context;

    public ProjectRequirementService(AppDbContext context)
    {
        _context = context;
    }

    public List<ProjectRequirementDto>
        GetRequirements(int projectId)
    {
        return _context.ProjectRequirements
            .Where(pr => pr.ProjectId == projectId)
            .Include(pr => pr.Skill)
            .Select(pr => new ProjectRequirementDto
            {
                SkillId = pr.SkillId,
                SkillName = pr.Skill.SkillName,
                RequiredLevel = pr.RequiredLevel
            })
            .ToList();
    }

    public bool AddRequirement(
        int projectId,
        CreateProjectRequirementDto dto)
    {
        var project =
            _context.Projects.Find(projectId);

        var skill =
            _context.Skills.Find(dto.SkillId);

        if (project is null || skill is null)
        {
            return false;
        }

        var exists =
            _context.ProjectRequirements.Any(pr =>
                pr.ProjectId == projectId &&
                pr.SkillId == dto.SkillId);

        if (exists)
        {
            return false;
        }

        _context.ProjectRequirements.Add(
            new ProjectRequirement
            {
                ProjectId = projectId,
                SkillId = dto.SkillId,
                RequiredLevel = dto.RequiredLevel
            });

        _context.SaveChanges();

        return true;
    }

    public bool RemoveRequirement(
        int projectId,
        int skillId)
    {
        var requirement =
            _context.ProjectRequirements
                .FirstOrDefault(pr =>
                    pr.ProjectId == projectId &&
                    pr.SkillId == skillId);

        if (requirement is null)
        {
            return false;
        }

        _context.ProjectRequirements.Remove(
            requirement);

        _context.SaveChanges();

        return true;
    }
}