using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
}