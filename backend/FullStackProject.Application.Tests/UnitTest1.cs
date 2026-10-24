
using FullStackProject.Application.Interfaces;
using FullStackProject.Application.Services;
using FullStackProject.Domain.Entities;
using Moq;
using Xunit;

namespace FullStackProject.Application.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private ProjectService CreateService()
    {
        return new ProjectService(
            _projectRepository.Object,
            _userRepository.Object,
            _currentUserService.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenUserIsNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _currentUserService
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetAllAsync());

        _projectRepository.Verify(
            x => x.GetForUserAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserHasNoAccess_ReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(userId);

        _projectRepository
            .Setup(x => x.HasAccessAsync(projectId, userId))
            .ReturnsAsync(false);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(projectId);

        // Assert
        Assert.Null(result);

        _projectRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var project = new Project
        {
            Id = projectId,
            Name = "Original project",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(otherUserId);

        _projectRepository
            .Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var service = CreateService();

        var request = new FullStackProject.Application.DTOs.UpdateProjectRequest
        {
            Name = "Modified project",
            Description = "Attempted unauthorized update"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.UpdateAsync(projectId, request));

        _projectRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Project>()),
            Times.Never);

        Assert.Equal("Original project", project.Name);
    }


    //Creating a project assigns the authenticated user as owner


    [Fact]
    public async Task CreateAsync_WhenUserExists_CreatesProjectOwnedByCurrentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "Owner",
            Email = "owner@example.com",
            PasswordHash = "test-hash"
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(userId);

        _userRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        Project? savedProject = null;

        _projectRepository
            .Setup(x => x.AddAsync(It.IsAny<Project>()))
            .Callback<Project>(project => savedProject = project)
            .Returns(Task.CompletedTask);

        var request =
            new FullStackProject.Application.DTOs.CreateProjectRequest
            {
                Name = "Unit Test Project",
                Description = "Created by a unit test"
            };

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(savedProject);
        Assert.Equal(userId, savedProject.OwnerId);
        Assert.Equal("Unit Test Project", result.Name);
        Assert.Equal(userId, result.OwnerId);
        Assert.Equal("Test Owner", result.OwnerName);

        _projectRepository.Verify(
            x => x.AddAsync(It.IsAny<Project>()),
            Times.Once);
    }

    //A non-owner cannot delete a project

    [Fact]
    public async Task DeleteAsync_WhenUserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var project = new Project
        {
            Id = projectId,
            Name = "Protected Project",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(otherUserId);

        _projectRepository
            .Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.DeleteAsync(projectId));

        _projectRepository.Verify(
            x => x.DeleteAsync(It.IsAny<Project>()),
            Times.Never);
    }


}
