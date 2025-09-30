using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RepositoryPattern.Controllers;
using RepositoryPattern.Models;
using RepositoryPattern.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoryPattern.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _controller = new UsersController(_userRepoMock.Object, null); // DbContext not needed
        }

        // GetAll
        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        {
            var users = new List<User> { new User { Id = 1, Name = "Test User" } };
            _userRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(users);
        }

        // GetById - exists
        [Fact]
        public async Task GetById_ReturnsOk_WhenUserExists()
        {
            var user = new User { Id = 1, Name = "Test User" };
            _userRepoMock.Setup(repo => repo.GetUserWithExpensesAsync(1)).ReturnsAsync(user);

            var result = await _controller.GetById(1);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().Be(user);
        }

        // GetById - does not exist
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(repo => repo.GetUserWithExpensesAsync(1)).ReturnsAsync((User)null);

            var result = await _controller.GetById(1);

            result.Should().BeOfType<NotFoundResult>();
        }

        // Create
        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenUserCreated()
        {
            var user = new User { Id = 1, Name = "New User" };
            _userRepoMock.Setup(repo => repo.AddAsync(user)).Returns(Task.CompletedTask);

            var result = await _controller.Create(user);

            var createdAtAction = result as CreatedAtActionResult;
            createdAtAction.Should().NotBeNull();
            createdAtAction.ActionName.Should().Be(nameof(_controller.GetById));
            createdAtAction.Value.Should().Be(user);

            _userRepoMock.Verify(repo => repo.AddAsync(user), Times.Once);
        }

        // Update - exists
        [Fact]
        public async Task Update_ReturnsNoContent_WhenUserExists()
        {
            var existingUser = new User { Id = 1, Name = "Old Name" };
            _userRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingUser);

            var updatedUser = new User { Id = 1, Name = "New Name" };

            var result = await _controller.Update(1, updatedUser);

            result.Should().BeOfType<NoContentResult>();
            existingUser.Name.Should().Be("New Name");
            _userRepoMock.Verify(repo => repo.Update(existingUser), Times.Once);
        }

        // Update - does not exist
        [Fact]
        public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

            var result = await _controller.Update(1, new User());

            result.Should().BeOfType<NotFoundResult>();
        }

        // Delete - exists
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenUserExists()
        {
            var existingUser = new User { Id = 1, Name = "Test User" };
            _userRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingUser);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NoContentResult>();
            _userRepoMock.Verify(repo => repo.Remove(existingUser), Times.Once);
        }

        // Delete - does not exist
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
