using Xunit;
using FluentAssertions;
using RepositoryPattern.Controllers;
using RepositoryPattern.Models;
using RepositoryPattern.Models.Data;
using RepositoryPattern.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RepositoryPattern.Tests
{
    public class UsersControllerIntegrationTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            return new AppDbContext(options);
        }

        private IUserRepository GetUserRepository(AppDbContext context) => new UserRepository(context);

        // GetAll Integration Test
        [Fact]
        public async Task GetAll_ReturnsAllUsers_FromInMemoryDb()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            await repo.AddAsync(new User { Name = "User 1" });
            await repo.AddAsync(new User { Name = "User 2" });

            var controller = new UsersController(repo, context); // pass both repo and context
            var result = await controller.GetAll();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var users = okResult.Value as IEnumerable<User>;
            users.Should().HaveCount(2);
        }

        // GetById Integration Test
        [Fact]
        public async Task GetById_ReturnsUser_WhenExists()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            var user = new User { Name = "User 1" };
            await repo.AddAsync(user);

            var controller = new UsersController(repo, context); // pass both
            var result = await controller.GetById(user.Id);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            var controller = new UsersController(repo, context); // pass both
            var result = await controller.GetById(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        // Create Integration Test
        [Fact]
        public async Task Create_AddsUser_ToInMemoryDb()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            var controller = new UsersController(repo, context); // pass both
            var user = new User { Name = "New User" };

            var result = await controller.Create(user);

            var createdAtAction = result as CreatedAtActionResult;
            createdAtAction.Should().NotBeNull();
            createdAtAction.Value.Should().BeEquivalentTo(user);

            // Verify user actually added to DB
            var dbUser = await context.Users.FindAsync(user.Id);
            dbUser.Should().NotBeNull();
            dbUser.Name.Should().Be("New User");
        }

        // Update Integration Test
        [Fact]
        public async Task Update_ModifiesUser_InInMemoryDb()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            var user = new User { Name = "Old Name" };
            await repo.AddAsync(user);

            var controller = new UsersController(repo, context); // pass both
            var updatedUser = new User { Name = "New Name" };

            var result = await controller.Update(user.Id, updatedUser);
            result.Should().BeOfType<NoContentResult>();

            var dbUser = await context.Users.FindAsync(user.Id);
            dbUser.Name.Should().Be("New Name");
        }

        // Delete Integration Test
        [Fact]
        public async Task Delete_RemovesUser_FromInMemoryDb()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetUserRepository(context);

            var user = new User { Name = "Delete Me" };
            await repo.AddAsync(user);

            var controller = new UsersController(repo, context); // pass both
            var result = await controller.Delete(user.Id);

            result.Should().BeOfType<NoContentResult>();

            var dbUser = await context.Users.FindAsync(user.Id);
            dbUser.Should().BeNull();
        }
    }
}
