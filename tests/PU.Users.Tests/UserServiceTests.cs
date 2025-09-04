using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PU.Users.Api.Data;
using PU.Users.Api.Models;
using PU.Users.Api.Repositories;
using PU.Users.Api.Services;
using Xunit;

namespace PU.Users.Tests;

public class UserServiceTests
{
    private AppDbContext BuildDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_And_Count_User()
    {
        using var db = BuildDb();

        // Create a mock of IUserRepository
        var mockRepo = new Mock<IUserRepository>();

        // Setup AddAsync to just return the user
        mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

        var svc = new UserService(db, mockRepo.Object);

        var u = await svc.CreateAsync(
            new User { FirstName = "Test", LastName = "User", Email = "t@u.com" },
            new int[0]
        );

        Assert.True(u.Id > 0);
        Assert.Equal(1, await svc.CountAsync());
    }
}
