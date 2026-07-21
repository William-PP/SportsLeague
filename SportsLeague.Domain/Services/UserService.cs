using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        _logger.LogInformation("Listing all users");
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        if (await _userRepository.ExistsByEmailAsync(user.Email))
        {
            _logger.LogWarning("Attempt to create user with duplicate email: {Email}", user.Email);
            throw new InvalidOperationException($"A user with email {user.Email} already exists");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _logger.LogInformation("Creating new user: {Email}", user.Email);
        return await _userRepository.CreateAsync(user);
    }

    public async Task UpdateAsync(int id, User user)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("User not found for update: {UserId}", id);
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.Role = user.Role;

        _logger.LogInformation("Updating user: {UserId}", id);
        await _userRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("User not found for deletion: {UserId}", id);
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        _logger.LogInformation("Deleting user: {UserId}", id);
        await _userRepository.DeleteAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
            return false;
        }

        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid)
        {
            _logger.LogWarning("Failed login attempt for: {Email}", email);
        }

        return isValid;
    }
}