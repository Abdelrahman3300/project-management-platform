using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullStackProject.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/members")]
[Authorize]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectMemberService _memberService;

    public ProjectMembersController(
        IProjectMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectMemberDto>>>
        GetMembers(Guid projectId)
    {
        var members =
            await _memberService.GetByProjectIdAsync(projectId);

        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectMemberDto>> AddMember(
        Guid projectId,
        [FromBody] AddProjectMemberRequest request)
    {
        try
        {
            var member =
                await _memberService.AddAsync(
                    projectId,
                    request.UserId,
                    request.Role);

            if (member is null)
            {
                return BadRequest(
                    "Project or user does not exist, or user is already a member.");
            }

            return Ok(member);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message = ex.Message
                });
        }
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid userId)
    {
        try
        {
            var removed =
                await _memberService.RemoveAsync(
                    projectId,
                    userId);

            if (!removed)
            {
                return NotFound(
                    "Project member was not found.");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message = ex.Message
                });
        }
    }
}