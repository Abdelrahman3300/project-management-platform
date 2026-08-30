
using FullStackProject.Application.DTOs;

namespace FullStackProject.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync();

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto> CreateAsync(CreateUserRequest request);
}
