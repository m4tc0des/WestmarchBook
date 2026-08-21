using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Infrastructure.DataAccess;

internal sealed class WestmarchBookDbContext : DbContext
{
    public WestmarchBookDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Users> Users { get; set; }
}
