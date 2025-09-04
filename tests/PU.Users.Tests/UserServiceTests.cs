using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;
using PU.Users.Api.Repositories;
using PU.Users.Api.Services;
using Xunit;

namespace PU.Users.Tests
{
    public class UserServiceTests
    {
        private AppDbContext BuildDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB for each test
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_And_Count_User()
        {
            // Arrange
            using var db = BuildDb();
            var repo = new UserRepository(db);
            var svc = new UserService(db, repo);

            var newUser = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "t@u.com"
            };

            // Act
            var createdUser = await svc.CreateAsync(newUser, new int[0]);
            var count = await svc.CountAsync();

            // Assert
            Assert.NotNull(createdUser);
            Assert.True(createdUser.Id > 0); // ID should be auto-generated
            Assert.Equal(1, count); // DB should now have exactly 1 user
        }
    }
}
