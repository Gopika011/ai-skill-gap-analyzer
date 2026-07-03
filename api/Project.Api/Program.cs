using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Dtos;
using Project.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=auth.db"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IEmployeeSkillService, EmployeeSkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectEmployeeService, ProjectEmployeeService>();
builder.Services.AddScoped<IProjectRequirementService, ProjectRequirementService>();
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
