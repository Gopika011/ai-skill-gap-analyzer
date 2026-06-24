using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Entities;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public List<Projectt> GetProjects()
    {
        return _context.Projects.ToList();
    }

    public Projectt CreateProject(CreateProjectDto dto)
    {
        var project = new Projectt
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        _context.Projects.Add(project);

        _context.SaveChanges();

        return project;
    }

    public bool DeleteProject(int projectId)
    {
        var project = _context.Projects.Find(projectId);

        if (project is null)
        {
            return false;
        }

        _context.Projects.Remove(project);

        _context.SaveChanges();

        return true;
    }
}