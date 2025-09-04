using Microsoft.EntityFrameworkCore;
using Moq;
using PU.Users.Api.Data;
using PU.Users.Api.Models;
using PU.Users.Api.Repositories;
using PU.Users.Api.Services;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;



namespace PU.Users.Tests;

public class UserServiceTests
{
    private AppDbContext BuildDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Can_Create_And_Count_Users()
    {
        using var db = BuildDb();

        // Mock IUserRepository
        var mockRepo = new Mock<IUserRepository>();

        // Pass both AppDbContext and IUserRepository to UserService
        var svc = new UserService(db, mockRepo.Object);

        var user = await svc.CreateAsync(
            new User { FirstName = "Test", LastName = "User", Email = "t@u.com" },
            Array.Empty<int>()
        );

        Assert.True(user.Id > 0);

        var count = await svc.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Can_Add_User_To_Group_And_List()
    {
        using var db = BuildDb();
        db.Groups.Add(new Group { Name = "Engineering" });
        await db.SaveChangesAsync();

        var mockRepo = new Mock<IUserRepository>();

        var uSvc = new UserService(db, mockRepo.Object);
        var gSvc = new GroupService(db);

        var u = await uSvc.CreateAsync(
            new User { FirstName = "A", LastName = "B", Email = "a@b.com" },
            Array.Empty<int>()
        );

        await gSvc.AddUserAsync(groupId: db.Groups.First().Id, userId: u.Id);

        var loaded = await uSvc.GetAsync(u.Id);
        Assert.Single(loaded!.UserGroups);
    }
}
