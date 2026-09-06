using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface IProjectMemberRepository
{
    Task<IReadOnlyList<ProjectMember>> GetByProjectIdAsync(Guid projectId);

    Task<ProjectMember?> GetAsync(Guid projectId, Guid userId);

    Task AddAsync(ProjectMember member);

    Task DeleteAsync(ProjectMember member);
}
