using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface ITaskItemRepository
{
    Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId);

    Task<TaskItem?> GetByIdAsync(Guid taskId);

    Task AddAsync(TaskItem task);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(TaskItem task);
}