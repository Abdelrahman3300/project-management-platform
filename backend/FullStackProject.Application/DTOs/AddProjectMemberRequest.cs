namespace FullStackProject.Application.DTOs;

public class AddProjectMemberRequest
{
    public Guid UserId { get; set; }

    public string Role { get; set; } = "Member";
}