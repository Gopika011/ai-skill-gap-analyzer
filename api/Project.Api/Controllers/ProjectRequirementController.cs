using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;

[ApiController]
[Route("projects/{projectId}/requirements")]
public class ProjectRequirementController
    : ControllerBase
{
    private readonly IProjectRequirementService
        _service;

    public ProjectRequirementController(
        IProjectRequirementService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetRequirements(
        int projectId)
    {
        return Ok(
            _service.GetRequirements(projectId));
    }

    [HttpPost]
    public IActionResult AddRequirement(
        int projectId,
        CreateProjectRequirementDto dto)
    {
        var added =
            _service.AddRequirement(
                projectId,
                dto);

        if (!added)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpDelete("{skillId}")]
    public IActionResult RemoveRequirement(
        int projectId,
        int skillId)
    {
        var removed =
            _service.RemoveRequirement(
                projectId,
                skillId);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}