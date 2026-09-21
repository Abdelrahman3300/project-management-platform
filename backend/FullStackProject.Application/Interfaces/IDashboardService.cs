using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}