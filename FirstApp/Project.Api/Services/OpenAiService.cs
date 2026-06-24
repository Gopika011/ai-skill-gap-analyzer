namespace Project.Api.Services;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] ?? "";
    }

    public async Task<string> AnalyzeSkillGapsAsync(string dataJson)
    {
        // Google Gemini Developer API endpoint
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        // input data for Gemini
        var systemInstruction = "You are an AI Skill Gap Analyzer. Analyze the project skill requirements against assigned employee skills. Group by employee, calculate gap severity (No Gap, Minor Gap, Moderate Gap, Critical Gap), and return a JSON object with: 1. 'summary' (overall health text), 2. 'gaps' (array of objects with EmployeeEmail, SkillName, RequiredLevel, ActualLevel, GapSeverity, and a short TrainingRecommendation).";
        
        var promptText = $"{systemInstruction}\n\nHere is the data in JSON format:\n{dataJson}";

        // Gemini payload structure
        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = promptText }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json" // Force JSON output
            }
        };

        var jsonContent = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        
        // Extract content from Gemini response structure: candidates[0].content.parts[0].text
        var content = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return content ?? "{}";
    }
}