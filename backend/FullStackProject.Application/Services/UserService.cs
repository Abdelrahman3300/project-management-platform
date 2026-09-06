using FullStackProject.Application.DTOs;
using FullStackProject.Application.Interfaces;
using BCrypt.Net;

namespace FullStackProject.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(MapToDto)
            .ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
{
    var emailExists =
        await _userRepository.ExistsByEmailAsync(request.Email);

    if (emailExists)
    {
        throw new InvalidOperationException(
            "A user with this email already exists.");
    }

    var user = new FullStackProject.Domain.Entities.User
    {
        Id = Guid.NewGuid(),
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        PasswordHash = BCrypt.HashPassword(request.Password),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    await _userRepository.AddAsync(user);

    return MapToDto(user);
}
   
    private static UserDto MapToDto(
        FullStackProject.Domain.Entities.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto?> UpdateAsync( Guid id, UpdateUserRequest request) { var user = await _userRepository.GetByIdAsync(id); if (user is null) { return null; } var existingUser = await _userRepository.GetByEmailAsync( request.Email); if (existingUser is not null && existingUser.Id != id) { throw new InvalidOperationException( "A user with this email already exists."); } user.FirstName = request.FirstName; user.LastName = request.LastName; user.Email = request.Email; user.IsActive = request.IsActive; await _userRepository.UpdateAsync(user); return MapToDto(user); }

    public async Task<bool> DeleteAsync(Guid id) { var user = await _userRepository.GetByIdAsync(id); if (user is null) { return false; } await _userRepository.DeleteAsync(user); return true; }
}
