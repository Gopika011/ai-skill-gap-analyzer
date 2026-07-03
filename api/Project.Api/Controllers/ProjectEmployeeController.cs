using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("projects/{projectId}/employees")]
public class ProjectEmployeeController : ControllerBase
{
    private readonly IProjectEmployeeService _service;

    public ProjectEmployeeController(IProjectEmployeeService service)
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
    public IActionResult AssignEmployee(int projectId, AssignEmployeeDto dto)
    {
        var assigned = _service.AssignEmployee(projectId, dto);
        if (!assigned)
        {
            return BadRequest("Could not assign employee. Check if employee is already assigned or doesn't exist.");
        }
        return Ok();
    }

    [HttpDelete("{employeeId}")]
    public IActionResult RemoveEmployee(int projectId, string employeeId)
    {
        var removed = _service.RemoveEmployee(projectId, employeeId);
        if (!removed)
        {
            return NotFound();
        }
        return NoContent();
    }
}