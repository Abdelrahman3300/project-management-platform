using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(Guid id);

    Task AddAsync(Project project);

    Task UpdateAsync(Project project);

    Task DeleteAsync(Project project);
}