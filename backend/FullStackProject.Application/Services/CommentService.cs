using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskItemRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public CommentService(
        ICommentRepository commentRepository,
        ITaskItemRepository taskRepository,
        ICurrentUserService currentUserService)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<CommentDto>> GetByTaskIdAsync(
        Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return Array.Empty<CommentDto>();

        var comments =
            await _commentRepository.GetByTaskIdAsync(taskId);

        return comments
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CommentDto?> AddAsync(
        Guid taskId,
        CreateCommentRequest request)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return null;

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content.Trim(),
            TaskItemId = taskId,
            UserId = currentUserId.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);

        var savedComment =
            await _commentRepository.GetByIdAsync(comment.Id);

        return savedComment is null
            ? null
            : MapToDto(savedComment);
    }

    public async Task<bool> DeleteAsync(Guid commentId)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user could not be identified.");
        }

        var comment =
            await _commentRepository.GetByIdAsync(commentId);

        if (comment is null)
            return false;

        if (comment.UserId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException(
                "Only the comment author can delete this comment.");
        }

        await _commentRepository.DeleteAsync(comment);

        return true;
    }

    private static CommentDto MapToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskItemId = comment.TaskItemId,
            UserId = comment.UserId,
            UserName =
                $"{comment.User.FirstName} {comment.User.LastName}",
            CreatedAt = comment.CreatedAt
        };
    }
}