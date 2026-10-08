namespace FullStackProject.Application.DTOs;

public class ProjectStatsDto
{
    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public int TotalTasks { get; set; }

    public int TodoTasks { get; set; }

    public int InProgressTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int CancelledTasks { get; set; }

    public int OverdueTasks { get; set; }

    public int CompletionPercentage { get; set; }
}