using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectMemberRepository _memberRepository;

    public TaskItemService(
        ITaskItemRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IProjectMemberRepository memberRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _memberRepository = memberRepository;
    }

    public async Task<IReadOnlyList<TaskItemDto>> GetByProjectIdAsync(
        Guid projectId)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

        return tasks
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TaskItemDto?> GetByIdAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return null;

        return MapToDto(task);
    }

    public async Task<TaskItemDto?> CreateAsync(
        Guid projectId,
        CreateTaskRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return null;

        if (request.AssignedToUserId.HasValue)
{
    var user = await _userRepository
        .GetByIdAsync(request.AssignedToUserId.Value);

    if (user is null)
        return null;

    var member = await _memberRepository.GetAsync(
        projectId,
        request.AssignedToUserId.Value);

    if (member is null)
        return null;
}

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
             DueDate = request.DueDate.HasValue ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc) : null,
            AssignedToUserId = request.AssignedToUserId
        };

        await _taskRepository.AddAsync(task);

        var createdTask = await _taskRepository.GetByIdAsync(task.Id);

        return createdTask is null
            ? null
            : MapToDto(createdTask);
    }

    public async Task<TaskItemDto?> UpdateAsync(
        Guid taskId,
        UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return null;

        if (request.AssignedToUserId.HasValue)
{
    var user = await _userRepository
        .GetByIdAsync(request.AssignedToUserId.Value);

    if (user is null)
        return null;

    var member = await _memberRepository.GetAsync(
        task.ProjectId,
        request.AssignedToUserId.Value);

    if (member is null)
        return null;
}

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate.HasValue
        ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc)
        : null;
        task.AssignedToUserId = request.AssignedToUserId;

        await _taskRepository.UpdateAsync(task);

        var updatedTask = await _taskRepository.GetByIdAsync(task.Id);

        return updatedTask is null
            ? null
            : MapToDto(updatedTask);
    }

    public async Task<bool> DeleteAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return false;

        await _taskRepository.DeleteAsync(task);

        return true;
    }

    private static TaskItemDto MapToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserName = task.AssignedToUser is null
                ? null
                : $"{task.AssignedToUser.FirstName} {task.AssignedToUser.LastName}"
        };
    }
}