
using FullStackProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullStackProject.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.OwnerId);

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.ProjectId);

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.AssignedToUserId);

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.TaskItemId);
    }
}
