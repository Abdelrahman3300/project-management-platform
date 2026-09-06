using FullStackProject.Domain.Enums;

namespace FullStackProject.Application.DTOs;

public class TaskItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public string? AssignedToUserName { get; set; }
}