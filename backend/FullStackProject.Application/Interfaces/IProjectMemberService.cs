using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface IProjectMemberService
{
    Task<IReadOnlyList<ProjectMemberDto>> GetByProjectIdAsync(Guid projectId);

    Task<ProjectMemberDto?> AddAsync(
        Guid projectId,
        Guid userId,
        string role);

    Task<bool> RemoveAsync(
        Guid projectId,
        Guid userId);
}