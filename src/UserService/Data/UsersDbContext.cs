using Microsoft.EntityFrameworkCore;
using Shared.Models;
using UserService.Models;

namespace UserService.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
}