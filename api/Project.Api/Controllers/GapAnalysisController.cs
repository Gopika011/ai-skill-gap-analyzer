using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Services;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Api.Controllers;

[ApiController]
[Route("gap-analysis")]
public class GapAnalysisController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IOpenAiService _openAiService;

    public GapAnalysisController(AppDbContext context, IOpenAiService openAiService)
    {
        _context = context;
        _openAiService = openAiService;
    }

    [HttpPost("project/{projectId}")]
    public async Task<IActionResult> RunAnalysis(int projectId)
    {
        // 1. Fetch project requirements
        var requirements = _context.ProjectRequirements
            .Where(pr => pr.ProjectId == projectId)
            .Select(pr => new { pr.Skill.SkillName, pr.RequiredLevel })
            .ToList();

        // 2. Fetch assigned employee IDs
        var assignedEmployeeIds = _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .Select(pe => pe.EmployeeId)
            .ToList();

        // 3. Fetch skills profile for those assigned employees (using Navigation Property Employee)
        var employeeSkills = _context.EmployeeSkill
            .Where(es => assignedEmployeeIds.Contains(es.EmployeeId))
            .Include(es => es.Skill)
            .Include(es => es.Employee)
            .Select(es => new
            {
                EmployeeEmail = es.Employee!.Email,
                SkillName = es.Skill!.SkillName,
                ProficiencyLevel = es.ProficiencyLevel
            })
            .ToList();

        // 4. Group employee skills by employee email
        var groupedEmployeeSkills = employeeSkills
            .GroupBy(es => es.EmployeeEmail)
            .Select(g => new
            {
                EmployeeEmail = g.Key,
                Skills = g.Select(s => new { s.SkillName, s.ProficiencyLevel }).ToList()
            })
            .ToList();

        // 5. Wrap everything in a data package for OpenAI
        var dataPackage = new
        {
            ProjectRequirements = requirements,
            EmployeesSkills = groupedEmployeeSkills
        };

        string rawJsonInput = JsonSerializer.Serialize(dataPackage);

        // 6. Send to AI
        try
        {
            var analysisResult = await _openAiService.AnalyzeSkillGapsAsync(rawJsonInput);
            return Content(analysisResult, "application/json");
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}