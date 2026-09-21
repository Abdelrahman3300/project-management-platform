using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync();
    
    Task<IReadOnlyList<Project>> GetForUserAsync(Guid userId);

    Task<Project?> GetByIdAsync(Guid id);

    Task<bool> HasAccessAsync(Guid projectId, Guid userId);
    
    Task AddAsync(Project project);

    Task UpdateAsync(Project project);

    Task DeleteAsync(Project project);

    
}