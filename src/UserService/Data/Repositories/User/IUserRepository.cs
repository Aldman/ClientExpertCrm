using Microsoft.EntityFrameworkCore.Storage;

namespace UserService.Data.Repositories.User;

public interface IUserRepository
{
    Task AddAsync(Models.User user, CancellationToken ct);
    Task<Models.User?> GetByUserNameAsync(string userName, CancellationToken ct);
    Task<Models.User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<Models.User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Models.User>> GetUsersAsync(int page, int pageSize, CancellationToken ct);
    Task UpdateAsync(Models.User user, CancellationToken ct);
    Task UpdateAsync(Guid userId, Models.User newUser, CancellationToken ct);
    Task DeleteAsync(Guid userId, CancellationToken ct);
    void Delete(Models.User user);
    Task<IDbContextTransaction> CreateTransactionAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}