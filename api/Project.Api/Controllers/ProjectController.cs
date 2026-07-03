using Microsoft.AspNetCore.Mvc;
using Project.Api.Dtos;
using Project.Api.Services;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetProjects()
    {
        return Ok(_service.GetProjects());
    }

    [HttpPost]
    public IActionResult CreateProject(CreateProjectDto dto)
    {
        return Ok(_service.CreateProject(dto));
    }

    [HttpDelete("{projectId}")]
    public IActionResult DeleteProject(int projectId)
    {
        var deleted = _service.DeleteProject(projectId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}