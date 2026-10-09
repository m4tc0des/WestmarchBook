using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WestmarchBook.Domain.Entities;

[assembly:InternalsVisibleTo("WebApi.Tests")]
namespace WestmarchBook.Infrastructure.DataAccess;

internal sealed class WestmarchBookDbContext : DbContext
{
    public WestmarchBookDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.BookId, ba.AuthorId });

        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookAuthors)
            .WithOne(ba => ba.Book)
            .HasForeignKey(ba => ba.BookId);

        modelBuilder.Entity<Author>()
            .HasMany(a => a.BookAuthors)
            .WithOne(ba => ba.Author)
            .HasForeignKey(ba => ba.AuthorId);
    }
}
