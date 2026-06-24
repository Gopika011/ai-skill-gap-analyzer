using Project.Api.Dtos;

namespace Project.Api.Services;

public interface IEmployeeSkillService
{
    List<EmployeeSkillResponseDto> GetEmployeeSkills(int userId);
    EmployeeSkillResponseDto? AddSkill(int userId, CreateEmployeeSkillDto dto);
    bool DeleteSkill(int userId, int skillId);
}