using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllAsync();

    Task<ProjectDto?> GetByIdAsync(Guid id);

    Task<ProjectDto> CreateAsync(CreateProjectRequest request);

    Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request);

    Task<bool> DeleteAsync(Guid id);
}
