using FullStackProject.Application.Interfaces;
using FullStackProject.Domain.Entities;
using FullStackProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStackProject.Infrastructure.Repositories;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProjectMember>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .Include(pm => pm.User)
            .FirstOrDefaultAsync(pm =>
                pm.ProjectId == projectId &&
                pm.UserId == userId);
    }

    public async Task AddAsync(ProjectMember member)
    {
        await _context.ProjectMembers.AddAsync(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProjectMember member)
    {
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}