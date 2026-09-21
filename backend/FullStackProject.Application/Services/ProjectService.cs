using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;


namespace FullStackProject.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync()
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

    return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var hasAccess =
            await _projectRepository.HasAccessAsync(
                id,
                currentUserId.Value);

        if (!hasAccess)
        {
            return null;
        }

        var project =
            await _projectRepository.GetByIdAsync(id);

        return project == null
            ? null
            : MapToDto(project);
    }

    

    public async Task<ProjectDto> CreateAsync(
        CreateProjectRequest request)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var owner =
            await _userRepository.GetByIdAsync(currentUserId.Value);

        if (owner == null)
        {
            throw new ArgumentException(
                "Authenticated user was not found.");
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            OwnerId = currentUserId.Value,
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
        var project =
            await _projectRepository.GetByIdAsync(id);

        if (project == null)
        {
            return null;
        }

        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.OwnerId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException(
                "Only the project owner can update this project.");
        }

        project.Name = request.Name;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);

        return MapToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project =
            await _projectRepository.GetByIdAsync(id);

        if (project == null)
        {
            return false;
        }

        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (project.OwnerId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException(
                "Only the project owner can delete this project.");
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