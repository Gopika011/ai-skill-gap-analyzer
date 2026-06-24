using Project.Api.Dtos;

namespace Project.Api.Services;

public interface ISkillService
{
    List<SkillResponseDto> GetSkills();

    SkillResponseDto? CreateSkill(CreateSkillDto dto);

    bool UpdateSkill(int skillId, CreateSkillDto dto);

    bool DeleteSkill(int skillId);
}