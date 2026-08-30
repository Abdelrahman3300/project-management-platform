
namespace FullStackProject.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();

    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();

    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
