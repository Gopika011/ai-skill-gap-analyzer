using Project.Api.Dtos;
using Project.Api.Entities;

public interface IProjectService
{
    List<Projectt> GetProjects();

    Projectt CreateProject(CreateProjectDto dto);

    bool DeleteProject(int projectId);
}