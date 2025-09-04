using PU.Users.Api.Models;

namespace PU.Users.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(int id);
    Task<List<User>> GetAllAsync(int page = 1, int pageSize = 50);
    Task<User> AddAsync(User user);
    Task DeleteAsync(User user);
    Task<int> CountAsync();
}
