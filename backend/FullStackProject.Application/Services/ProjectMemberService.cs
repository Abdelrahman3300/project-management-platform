using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Services;

public class ProjectMemberService : IProjectMemberService
{
    private readonly IProjectMemberRepository _memberRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProjectMemberService(
        IProjectMemberRepository memberRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _memberRepository = memberRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<ProjectMemberDto>>
        GetByProjectIdAsync(Guid projectId)
    {
        var members =
            await _memberRepository.GetByProjectIdAsync(projectId);

        return members.Select(MapToDto).ToList();
    }

    public async Task<ProjectMemberDto?> AddAsync(
        Guid projectId,
        Guid userId,
        string role)
    {
        var project =
            await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return null;
        }

        var currentUserId =
            _currentUserService.UserId;

        if (currentUserId is null ||
            project.OwnerId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException(
                "Only the project owner can add members.");
        }

        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        var existingMember =
            await _memberRepository.GetAsync(
                projectId,
                userId);

        if (existingMember is not null)
        {
            return null;
        }

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = string.IsNullOrWhiteSpace(role)
                ? "Member"
                : role
        };

        await _memberRepository.AddAsync(member);

        return new ProjectMemberDto
        {
            ProjectId = member.ProjectId,
            UserId = member.UserId,
            UserName =
                $"{user.FirstName} {user.LastName}",
            Role = member.Role,
            JoinedAt = member.JoinedAt
        };
    }

    public async Task<bool> RemoveAsync(
        Guid projectId,
        Guid userId)
    {
        var project =
            await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return false;
        }

        var currentUserId =
            _currentUserService.UserId;

        if (currentUserId is null ||
            project.OwnerId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException(
                "Only the project owner can remove members.");
        }

        var member =
            await _memberRepository.GetAsync(
                projectId,
                userId);

        if (member is null)
        {
            return false;
        }

        await _memberRepository.DeleteAsync(member);

        return true;
    }

    private static ProjectMemberDto MapToDto(
        ProjectMember member)
    {
        return new ProjectMemberDto
        {
            ProjectId = member.ProjectId,
            UserId = member.UserId,
            UserName =
                $"{member.User.FirstName} {member.User.LastName}",
            Role = member.Role,
            JoinedAt = member.JoinedAt
        };
    }
}