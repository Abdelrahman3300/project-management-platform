
namespace FullStackProject.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid TaskItemId { get; set; }

    public Guid UserId { get; set; }

    // Relationships
    public TaskItem TaskItem { get; set; } = null!;

    public User User { get; set; } = null!;
}
