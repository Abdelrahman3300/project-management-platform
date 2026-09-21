namespace FullStackProject.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}