using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user, string password);
    Task UpdateAsync(int id, User user);
    Task DeleteAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ValidatePasswordAsync(string email, string password);
}