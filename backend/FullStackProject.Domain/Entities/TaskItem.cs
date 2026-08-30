
using FullStackProject.Domain.Enums;

namespace FullStackProject.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    // Relationships
    public Project Project { get; set; } = null!;

    public User? AssignedToUser { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
