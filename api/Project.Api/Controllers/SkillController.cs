using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("skills")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public IActionResult GetSkills()
    {
        return Ok(_skillService.GetSkills());
    }

    [HttpPost]
    public IActionResult CreateSkill(CreateSkillDto dto)
    {
        var skill = _skillService.CreateSkill(dto);

        if (skill is null)
        {
            return BadRequest("Category not found");
        }

        return Ok(skill);
    }

    [HttpPut("{skillId}")]
    public IActionResult UpdateSkill(int skillId, CreateSkillDto dto)
    {
        var updated = _skillService.UpdateSkill(skillId, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{skillId}")]
    public IActionResult DeleteSkill(int skillId)
    {
        var deleted = _skillService.DeleteSkill(skillId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}