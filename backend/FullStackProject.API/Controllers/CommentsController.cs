using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullStackProject.API.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetComments(
        Guid taskId)
    {
        var comments =
            await _commentService.GetByTaskIdAsync(taskId);

        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(
        Guid taskId,
        [FromBody] CreateCommentRequest request)
    {
        try
        {
            var comment =
                await _commentService.AddAsync(taskId, request);

            if (comment is null)
                return NotFound("Task was not found.");

            return Ok(comment);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpDelete("/api/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        try
        {
            var deleted =
                await _commentService.DeleteAsync(commentId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
    }
}