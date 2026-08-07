using Microsoft.Extensions.Logging;
using Moq;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Services;

namespace SportsLeague.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        var logger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepo.Object, logger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ThrowsException()
    {
        _mockRepo.Setup(r => r.ExistsByEmailAsync("dupe@test.com"))
                 .ReturnsAsync(true);

        var user = new User { Email = "dupe@test.com", FirstName = "A", LastName = "B", Role = UserRole.Viewer };

        var act = () => _service.CreateAsync(user, "Passw0rd");

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_HashesPassword()
    {
        _mockRepo.Setup(r => r.ExistsByEmailAsync("new@test.com"))
                 .ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => u);

        var user = new User { Email = "new@test.com", FirstName = "A", LastName = "B", Role = UserRole.Viewer };

        var result = await _service.CreateAsync(user, "Passw0rd");

        Assert.NotEqual("Passw0rd", result.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Passw0rd", result.PasswordHash));
    }

    [Fact]
    public async Task ValidatePasswordAsync_WrongPassword_ReturnsFalse()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync("user@test.com"))
                 .ReturnsAsync(new User { Email = "user@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct1") });

        var result = await _service.ValidatePasswordAsync("user@test.com", "Wrong1");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePasswordAsync_CorrectPassword_ReturnsTrue()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync("user@test.com"))
                 .ReturnsAsync(new User { Email = "user@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct1") });

        var result = await _service.ValidatePasswordAsync("user@test.com", "Correct1");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_UpdatesFields()
    {
        var existing = new User { Id = 1, Email = "old@test.com", FirstName = "Old", LastName = "Name", Role = UserRole.Viewer };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var user = new User { Email = "new@test.com", FirstName = "New", LastName = "Name", Role = UserRole.Admin };

        await _service.UpdateAsync(1, user);

        _mockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
        Assert.Equal("new@test.com", existing.Email);
        Assert.Equal("New", existing.FirstName);
        Assert.Equal(UserRole.Admin, existing.Role);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentUser_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var user = new User { Email = "ghost@test.com", FirstName = "G", LastName = "H", Role = UserRole.Viewer };

        var act = () => _service.UpdateAsync(99, user);

        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_Deletes()
    {
        var existing = new User { Id = 1, Email = "del@test.com", FirstName = "D", LastName = "E", Role = UserRole.Viewer };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _service.DeleteAsync(1);

        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentUser_ThrowsException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var act = () => _service.DeleteAsync(99);

        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithNonExistentEmail_ReturnsFalse()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync("ghost@test.com")).ReturnsAsync((User?)null);

        var result = await _service.ValidatePasswordAsync("ghost@test.com", "Passw0rd");

        Assert.False(result);
    }
}