using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;
using FullStackProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStackProject.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(x => x.Owner)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Project>> GetForUserAsync(
        Guid userId)
    {
        return await _context.Projects
            .Include(x => x.Owner)
            .Where(x =>
                x.OwnerId == userId ||
                x.Members.Any(m => m.UserId == userId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasAccessAsync(
    Guid projectId,
    Guid userId)
    {
        return await _context.Projects
            .AnyAsync(x =>
                x.Id == projectId &&
                (
                    x.OwnerId == userId ||
                    x.Members.Any(m => m.UserId == userId)
                ));
    }
}