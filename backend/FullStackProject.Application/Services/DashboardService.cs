using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Enums;
using FullStackProject.Application.Exceptions;

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
            throw new UnauthenticatedUserException();
        }

        var projects =
            await _projectRepository.GetForUserAsync(
                currentUserId.Value);

        var projectStats = new List<ProjectStatsDto>();

        // Overall dashboard totals
        var totalTasks = 0;
        var todoTasks = 0;
        var inProgressTasks = 0;
        var completedTasks = 0;
        var cancelledTasks = 0;
        var overdueTasks = 0;

        foreach (var project in projects)
        {
            var tasks =
                await _taskRepository.GetByProjectIdAsync(
                    project.Id);

            var projectTotalTasks = tasks.Count;

            var projectTodoTasks = tasks.Count(
                x => x.Status == TaskItemStatus.Todo);

            var projectInProgressTasks = tasks.Count(
                x => x.Status == TaskItemStatus.InProgress);

            var projectCompletedTasks = tasks.Count(
                x => x.Status == TaskItemStatus.Completed);

            var projectCancelledTasks = tasks.Count(
                x => x.Status == TaskItemStatus.Cancelled);

            var projectOverdueTasks = tasks.Count(IsOverdue);

            var completionPercentage = projectTotalTasks == 0
                ? 0
                : (int)Math.Round(
                    projectCompletedTasks * 100.0 /
                    projectTotalTasks);

            var stats = new ProjectStatsDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,

                TotalTasks = projectTotalTasks,

                TodoTasks = projectTodoTasks,

                InProgressTasks = projectInProgressTasks,

                CompletedTasks = projectCompletedTasks,

                CancelledTasks = projectCancelledTasks,

                OverdueTasks = projectOverdueTasks,

                CompletionPercentage = completionPercentage
            };

            projectStats.Add(stats);

            // Add this project's numbers to the overall totals
            totalTasks += projectTotalTasks;
            todoTasks += projectTodoTasks;
            inProgressTasks += projectInProgressTasks;
            completedTasks += projectCompletedTasks;
            cancelledTasks += projectCancelledTasks;
            overdueTasks += projectOverdueTasks;
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