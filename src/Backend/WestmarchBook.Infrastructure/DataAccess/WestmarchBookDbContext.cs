using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WestmarchBook.Domain.Entities;

[assembly:InternalsVisibleTo("WebApi.Tests")]
namespace WestmarchBook.Infrastructure.DataAccess;

internal sealed class WestmarchBookDbContext : DbContext
{
    public WestmarchBookDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
