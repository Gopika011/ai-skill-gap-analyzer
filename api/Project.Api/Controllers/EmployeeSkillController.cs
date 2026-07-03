using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("employees/{employeeId}/skills")]
public class EmployeeSkillController : ControllerBase
{
    private readonly IEmployeeSkillService _service;

    public EmployeeSkillController(IEmployeeSkillService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSkills(string employeeId) =>
        Ok(_service.GetEmployeeSkills(employeeId));

    [HttpPost]
    public IActionResult AddSkill(string employeeId, CreateEmployeeSkillDto dto)
    {
        var result = _service.AddSkill(employeeId, dto);
        if (result is null) 
        {
            return BadRequest("Employee or Skill not found");
        }
        return CreatedAtAction(nameof(GetSkills), new { employeeId }, result);
    }

    [HttpDelete("{skillId}")]
    public IActionResult DeleteSkill(string employeeId, int skillId)
    {
        var deleted = _service.DeleteSkill(employeeId, skillId);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}