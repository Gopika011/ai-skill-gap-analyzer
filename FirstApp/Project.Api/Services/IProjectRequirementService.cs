using Project.Api.Dtos;

public interface IProjectRequirementService
{
    List<ProjectRequirementDto> GetRequirements(int projectId);

    bool AddRequirement(int projectId, CreateProjectRequirementDto dto);

    bool RemoveRequirement(int projectId, int skillId);
}