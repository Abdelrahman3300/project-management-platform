using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        return project == null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId);

        if (owner == null)
        {
            throw new ArgumentException("Owner user not found.");
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.OwnerId,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project);

        project.Owner = owner;

        return MapToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(
        Guid id,
        UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
        {
            return null;
        }

        project.Name = request.Name;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);

        return MapToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
        {
            return false;
        }

        await _projectRepository.DeleteAsync(project);

        return true;
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = project.Owner != null
                ? $"{project.Owner.FirstName} {project.Owner.LastName}"
                : string.Empty,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
