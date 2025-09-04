using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Models;

namespace PU.Users.Api.Services;

public class PermissionService
{
    private readonly AppDbContext _db;
    public PermissionService(AppDbContext db) => _db = db;

    public async Task<List<Permission>> GetAllAsync() =>
        await _db.Permissions.AsNoTracking().ToListAsync();
}
