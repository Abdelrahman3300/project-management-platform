using System.Security.Claims;
using FullStackProject.Application.Interfaces;

namespace FullStackProject.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out var id))
            {
                return id;
            }

            return null;
        }
    }
}