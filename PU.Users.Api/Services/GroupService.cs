using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;

namespace PU.Users.Api.Services;

public class GroupService
{
    private readonly AppDbContext _db;
    public GroupService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Group>> GetAllAsync() =>
        await _db.Groups.AsNoTracking().ToListAsync();

    public async Task<Group> CreateAsync(string name, int[] permissionIds)
    {
        var g = new Group { Name = name };
        _db.Groups.Add(g);
        await _db.SaveChangesAsync();

        foreach (var pid in permissionIds.Distinct())
            _db.GroupPermissions.Add(new GroupPermission { GroupId = g.Id, PermissionId = pid });

        await _db.SaveChangesAsync();
        return g;
    }

    public async Task AddUserAsync(int groupId, int userId)
    {
        if (!await _db.UserGroups.AnyAsync(x => x.GroupId == groupId && x.UserId == userId))
        {
            _db.UserGroups.Add(new UserGroup { GroupId = groupId, UserId = userId });
            await _db.SaveChangesAsync();
        }
    }

    public async Task RemoveUserAsync(int groupId, int userId)
    {
        var rel = await _db.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
        if (rel != null)
        {
            _db.UserGroups.Remove(rel);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<int, int>> UsersPerGroupAsync()
    {
        return await _db.Groups
            .Select(g => new { g.Id, Count = g.UserGroups.Count })
            .ToDictionaryAsync(x => x.Id, x => x.Count);
    }
}
