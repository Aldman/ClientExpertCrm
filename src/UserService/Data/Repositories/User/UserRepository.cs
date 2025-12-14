using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UserService.Models;

namespace UserService.Data.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Models.User user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
    }

    public async Task<Models.User?> GetByUserNameAsync(string userName, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
    }

    public async Task<Models.User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<Models.User?> GetByIdAsync(Guid id, CancellationToken ct)
    { 
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<IEnumerable<Models.User>> GetUsersAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _dbContext.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Models.User user, CancellationToken ct)
    {
        var toEdit = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, ct);
        toEdit?.Change(user);
    }

    public async Task UpdateAsync(Guid userId, Models.User newUser, CancellationToken ct)
    {
        newUser.Id = userId;
        await UpdateAsync(newUser, ct);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstAsync(u => u.Id == userId, ct);
        Delete(user);
    }

    public void Delete(Models.User user)
    {
        _dbContext.Users.Remove(user);
    }

    public async Task<IDbContextTransaction> CreateTransactionAsync(CancellationToken ct)
    {
        return await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}