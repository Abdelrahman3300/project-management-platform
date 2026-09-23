using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface ICommentRepository
{
    Task<IReadOnlyList<Comment>> GetByTaskIdAsync(Guid taskId);
    Task<Comment?> GetByIdAsync(Guid commentId);
    Task AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}