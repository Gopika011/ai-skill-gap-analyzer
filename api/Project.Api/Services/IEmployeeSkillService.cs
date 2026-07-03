using Project.Api.Dtos;

namespace Project.Api.Services;

public interface IEmployeeSkillService
{
    List<EmployeeSkillResponseDto> GetEmployeeSkills(string employeeId);
    EmployeeSkillResponseDto? AddSkill(string employeeId, CreateEmployeeSkillDto dto);
    bool DeleteSkill(string employeeId, int skillId);
}