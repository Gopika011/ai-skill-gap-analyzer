using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

[ApiController]
[Route("employees/{userId}/skills")]
public class EmployeeSkillController : ControllerBase
{
    private readonly IEmployeeSkillService _service;
    #region CONSTRUCTOR 
    /// <summary>
    /// Creates a new EmployeeSkillController.
    /// </summary>
    /// <param name="service">
    /// Service used to manage employee skills.
    /// </param>
    public EmployeeSkillController(IEmployeeSkillService service)
    {
        _service = service;
    }
    #endregion


    #region PUBLIC METHODS
    /// <summary>
    /// Return all skills of the employee
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetSkills(int userId) =>
        Ok(_service.GetEmployeeSkills(userId));



    /// <summary>
    /// Adds a skill to the employee
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult AddSkill(int userId, CreateEmployeeSkillDto dto)
    {
        var result = _service.AddSkill(userId, dto);
        if (result is null) 
        {
            return BadRequest("User or Skill not found");
        }
        return CreatedAtAction(nameof(GetSkills), new { userId }, result);
    }


    /// <summary>
    /// Deletes an employee skill
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    [HttpDelete("{skillId}")]
    public IActionResult DeleteSkill(int userId, int skillId)
    {
        var deleted = _service.DeleteSkill(userId, skillId);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    #endregion
}