using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface ITaskItemService
{
    Task<IReadOnlyList<TaskItemDto>> GetByProjectIdAsync(
        Guid projectId);

    Task<TaskItemDto?> GetByIdAsync(
        Guid taskId);

    Task<TaskItemDto?> CreateAsync(
        Guid projectId,
        CreateTaskRequest request);

    Task<TaskItemDto?> UpdateAsync(
        Guid taskId,
        UpdateTaskRequest request);

    Task<bool> DeleteAsync(
        Guid taskId);
}