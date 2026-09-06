using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;
using FullStackProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStackProject.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;

    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(
        Guid projectId)
    {
        return await _context.TaskItems
            .Include(t => t.AssignedToUser)
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId)
    {
        return await _context.TaskItems
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
    }
}