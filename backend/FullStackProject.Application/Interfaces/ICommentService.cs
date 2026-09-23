using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface ICommentService
{
    Task<IReadOnlyList<CommentDto>> GetByTaskIdAsync(Guid taskId);

    Task<CommentDto?> AddAsync(
        Guid taskId,
        CreateCommentRequest request);

    Task<bool> DeleteAsync(Guid commentId);
}