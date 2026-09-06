namespace FullStackProject.Application.DTOs;

public class ProjectMemberDto
{
    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Role { get; set; } = "Member";

    public DateTime JoinedAt { get; set; }
}