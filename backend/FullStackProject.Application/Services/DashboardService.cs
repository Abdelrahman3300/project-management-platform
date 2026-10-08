using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Enums;

namespace FullStackProject.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskItemRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(
        IProjectRepository projectRepository,
        ITaskItemRepository taskRepository,
        ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var projects =
            await _projectRepository.GetForUserAsync(
                currentUserId.Value);

        var projectStats = new List<ProjectStatsDto>();

        var totalTasks = 0;
        var todoTasks = 0;
        var inProgressTasks = 0;
        var completedTasks = 0;
        var cancelledTasks = 0;
        var overdueTasks = 0;

        foreach (var project in projects)
        {
            var totalTasks = tasks.Count;

            var completedTasks = tasks.Count(
                x => x.Status == TaskItemStatus.Completed);

            var completionPercentage = totalTasks == 0
                ? 0
                : (int)Math.Round(
                    completedTasks * 100.0 / totalTasks);

            var stats = new ProjectStatsDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,

                TotalTasks = totalTasks,

                TodoTasks = tasks.Count(
                    x => x.Status == TaskItemStatus.Todo),

                InProgressTasks = tasks.Count(
                    x => x.Status == TaskItemStatus.InProgress),

                CompletedTasks = completedTasks,

                CancelledTasks = tasks.Count(
                    x => x.Status == TaskItemStatus.Cancelled),

                OverdueTasks = projectOverdue,

                CompletionPercentage = completionPercentage
            };
        }

        return new DashboardStatsDto
        {
            TotalProjects = projects.Count,

            TotalTasks = totalTasks,

            TodoTasks = todoTasks,

            InProgressTasks = inProgressTasks,

            CompletedTasks = completedTasks,

            CancelledTasks = cancelledTasks,

            OverdueTasks = overdueTasks,

            Projects = projectStats
        };
    }

    private static bool IsOverdue(
        FullStackProject.Domain.Entities.TaskItem task)
    {
        return task.DueDate.HasValue
            && task.DueDate.Value < DateTime.UtcNow
            && task.Status != TaskItemStatus.Completed
            && task.Status != TaskItemStatus.Cancelled;
    }
}