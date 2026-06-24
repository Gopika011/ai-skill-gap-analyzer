namespace Project.Api.Services;

using System.Threading.Tasks;

public interface IOpenAiService
{
    Task<string> AnalyzeSkillGapsAsync(string promptJson);
}