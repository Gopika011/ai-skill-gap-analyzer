namespace Project.Api.Entities;

public class EmployeeSkill
{
    public int Id { get; set; }

    // FK → Users
    public int UserId { get; set; }

    // FK → Skills
    public int SkillId { get; set; }

    public required string ProficiencyLevel { get; set; } // Beginner / Intermediate / Advanced / Expert

    public int YearsOfExperience { get; set; }

    // Navigation properties
    public Skill? Skill { get; set; }

    public User? User { get; set; }
}