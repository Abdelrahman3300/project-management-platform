using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FullStackProject.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/members")]
[Authorize]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectMemberService _memberService;

    public ProjectMembersController(IProjectMemberService memberService)
    {
        _memberService = memberService;
    }

    // GET: api/projects/{projectId}/members
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectMemberDto>>> GetMembers(
        Guid projectId)
    {
        var members = await _memberService.GetByProjectIdAsync(projectId);

        return Ok(members);
    }

    // POST: api/projects/{projectId}/members
    [HttpPost]
    public async Task<ActionResult<ProjectMemberDto>> AddMember(
        Guid projectId,
        [FromBody] AddProjectMemberRequest request)
    {
        var member = await _memberService.AddAsync(
            projectId,
            request.UserId,
            request.Role);

        if (member is null)
        {
            return BadRequest("Project or user does not exist, or user is already a member.");
        }

        return Ok(member);
    }

    // DELETE: api/projects/{projectId}/members/{userId}
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid userId)
    {
        var removed = await _memberService.RemoveAsync(
            projectId,
            userId);

        if (!removed)
        {
            return NotFound("Project member was not found.");
        }

        return NoContent();
    }
}