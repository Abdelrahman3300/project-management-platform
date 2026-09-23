using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;
using FullStackProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStackProject.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Comment>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TaskItemId == taskId)
            .OrderBy(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid commentId)
    {
        return await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task AddAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}