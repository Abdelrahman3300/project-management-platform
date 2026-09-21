namespace FullStackProject.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalProjects { get; set; }

    public int TotalTasks { get; set; }

    public int TodoTasks { get; set; }

    public int InProgressTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int CancelledTasks { get; set; }

    public int OverdueTasks { get; set; }

    public IReadOnlyList<ProjectStatsDto> Projects { get; set; }
        = new List<ProjectStatsDto>();
}