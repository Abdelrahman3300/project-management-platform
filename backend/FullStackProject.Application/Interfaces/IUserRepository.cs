
using FullStackProject.Domain.Entities;

namespace FullStackProject.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<IReadOnlyList<User>> GetAllAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);
    
    Task DeleteAsync(User user);

    Task<bool> ExistsByEmailAsync(string email);
}
