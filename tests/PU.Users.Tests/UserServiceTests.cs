using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;
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
        var svc = new UserService(db);
        var u = await svc.CreateAsync(new User{ FirstName="Test", LastName="User", Email="t@u.com"}, Array.Empty<int>());
        Assert.True(u.Id > 0);
        Assert.Equal(1, await svc.CountAsync());
    }
}
