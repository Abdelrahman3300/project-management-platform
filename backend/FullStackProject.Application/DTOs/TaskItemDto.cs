using System.ComponentModel.DataAnnotations;
using FullStackProject.Domain.Enums;
using FullStackProject.Application.Validation;

namespace FullStackProject.Application.DTOs;

public class TaskItemDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Task title is required.")]
    [NotWhitespace]
    [StringLength(150, MinimumLength = 2,ErrorMessage = "Task title must be between 2 and 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000,ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public string? AssignedToUserName { get; set; }
}