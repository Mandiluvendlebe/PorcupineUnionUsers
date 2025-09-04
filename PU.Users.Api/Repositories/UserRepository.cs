using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;

namespace PU.Users.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetAsync(int id) =>
        await _db.Users.Include(u => u.UserGroups).ThenInclude(ug => ug.Group).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<List<User>> GetAllAsync(int page = 1, int pageSize = 50) =>
        await _db.Users.Include(u => u.UserGroups).ThenInclude(ug => ug.Group)
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public Task<int> CountAsync() => _db.Users.CountAsync();
}
