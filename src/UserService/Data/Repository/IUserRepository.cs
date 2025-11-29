using UserService.Models;

namespace UserService.Data.Repository;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task UpdateAsync(Guid userId, User newUser, CancellationToken ct);
    Task DeleteAsync(Guid userId, CancellationToken ct);
    void Delete(User user);
    Task SaveChangesAsync(CancellationToken ct);
}