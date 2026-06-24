using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;

[ApiController]
[Route("projects/{projectId}/employees")]
public class ProjectEmployeeController : ControllerBase
{
    private readonly IProjectEmployeeService _service;

    public ProjectEmployeeController(
        IProjectEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetEmployees(int projectId)
    {
        var employees = _service.GetEmployees(projectId);

        return Ok(employees);
    }

    [HttpPost]
    public IActionResult AssignEmployee(
        int projectId,
        AssignEmployeeDto dto)
    {
        var assigned =
            _service.AssignEmployee(projectId, dto);

        if (!assigned)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpDelete("{userId}")]
    public IActionResult RemoveEmployee(
        int projectId,
        int userId)
    {
        var removed =
            _service.RemoveEmployee(projectId, userId);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}