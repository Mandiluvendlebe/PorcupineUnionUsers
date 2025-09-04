using PU.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace PU.Users.Api.Data;

public static class SeedData
{
    public static async Task EnsureSeedAsync(AppDbContext db)
    {
        if (await db.Groups.AnyAsync()) return;

        var groups = new[] {
            new Group { Name = "Engineering" },
            new Group { Name = "Human Resources" },
            new Group { Name = "Operations" }
        };

        var permissions = new[] {
            new Permission { Name = "Level 1" },
            new Permission { Name = "Level 2" },
            new Permission { Name = "Level 3" }
        };

        db.Groups.AddRange(groups);
        db.Permissions.AddRange(permissions);
        await db.SaveChangesAsync();

        // Group-Permissions
        db.GroupPermissions.AddRange(new[] {
            new GroupPermission { GroupId = groups[0].Id, PermissionId = permissions[0].Id },
            new GroupPermission { GroupId = groups[0].Id, PermissionId = permissions[1].Id },
            new GroupPermission { GroupId = groups[1].Id, PermissionId = permissions[0].Id },
            new GroupPermission { GroupId = groups[2].Id, PermissionId = permissions[2].Id }
        });

        // Users
        var users = new[] {
            new User { FirstName = "Alice", LastName = "Ngwenya", Email = "alice@example.com" },
            new User { FirstName = "Bongani", LastName = "Mokoena", Email = "bongani@example.com" },
            new User { FirstName = "Charmaine", LastName = "Pillay", Email = "charmaine@example.com" }
        };
        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        // User-Groups
        db.UserGroups.AddRange(new[] {
            new UserGroup { UserId = users[0].Id, GroupId = groups[0].Id },
            new UserGroup { UserId = users[1].Id, GroupId = groups[1].Id },
            new UserGroup { UserId = users[1].Id, GroupId = groups[2].Id },
            new UserGroup { UserId = users[2].Id, GroupId = groups[0].Id }
        });

        await db.SaveChangesAsync();
    }
}
