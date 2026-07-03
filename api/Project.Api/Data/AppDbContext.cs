using Microsoft.EntityFrameworkCore;
using Project.Api.Entities;

namespace Project.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectEmployee>()
            .HasKey(pe => new
            {
                pe.ProjectId,
                pe.EmployeeId
            });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Skill> Skills { get; set; }

    public DbSet<EmployeeSkill> EmployeeSkill { get; set; }

    public DbSet<Projectt> Projects => Set<Projectt>();

    public DbSet<ProjectEmployee> ProjectEmployees => Set<ProjectEmployee>();

    public DbSet<ProjectRequirement> ProjectRequirements => Set<ProjectRequirement>();
}