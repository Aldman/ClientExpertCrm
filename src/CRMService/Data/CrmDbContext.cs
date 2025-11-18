using CRMService.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMService.Data;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
}