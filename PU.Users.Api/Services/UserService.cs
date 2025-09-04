using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;

namespace PU.Users.Api.Services;

using PU.Users.Api.Repositories;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly IUserRepository _repo;

    public UserService(AppDbContext db, IUserRepository repo) { _db = db; _repo = repo; }

    public async Task<List<User>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        return await _repo.GetAllAsync(page, pageSize);
    }

    public async Task<User?> GetAsync(int id)
    {
        return await _repo.GetAsync(id);
    }

    public async Task<User> CreateAsync(User user, int[] groupIds)
    {
        var created = await _repo.AddAsync(user);
        foreach (var gid in groupIds.Distinct())
            _db.UserGroups.Add(new UserGroup { UserId = created.Id, GroupId = gid });
        await _db.SaveChangesAsync();
        return created;
    }

    public async Task<bool> UpdateAsync(int id, User updated, int[] groupIds)
    {
        var entity = await _db.Users.Include(u => u.UserGroups).FirstOrDefaultAsync(u => u.Id == id);
        if (entity is null) return false;

        entity.FirstName = updated.FirstName;
        entity.LastName = updated.LastName;
        entity.Email = updated.Email;

        // sync groups
        var current = entity.UserGroups.Select(x => x.GroupId).ToHashSet();
        var desired = groupIds.Distinct().ToHashSet();

        foreach (var toRemove in current.Except(desired).ToList())
            _db.UserGroups.Remove(new UserGroup { UserId = id, GroupId = toRemove });

        foreach (var toAdd in desired.Except(current))
            _db.UserGroups.Add(new UserGroup { UserId = id, GroupId = toAdd });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity is null) return false;
        await _repo.DeleteAsync(entity);
        return true;
    }

    public Task<int> CountAsync() => _repo.CountAsync();
}
