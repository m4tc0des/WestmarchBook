using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Infrastructure.Converters;

[assembly: InternalsVisibleTo("WebApi.Tests")]
namespace WestmarchBook.Infrastructure.DataAccess;

internal sealed class WestmarchBookDbContext : DbContext
{
    public WestmarchBookDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<Reader> Readers { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

        var genreComparer = new ValueComparer<ICollection<Genre>>(
            (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder.Entity<Book>()
            .Property(b => b.Genres)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptionsHelper.Default),
                v => JsonSerializer.Deserialize<ICollection<Genre>>(v, JsonOptionsHelper.Default)!,
                genreComparer);

        modelBuilder.Entity<Book>()
            .HasMany(copy => copy.BookCopies)
            .WithOne(book => book.Book)
            .HasForeignKey(fk => fk.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookCopy>()
            .HasIndex(rg => rg.RegistrationNumber)
            .IsUnique();

        modelBuilder.Entity<Rent>()
            .Property(r => r.RentStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(255);

            entity.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VerificationCode>()
        .Property(code => code.Type)
        .HasConversion<string>();

        modelBuilder.Entity<VerificationCode>()
            .HasOne<User>().WithMany()
            .HasForeignKey(code => code.UserId);

        modelBuilder.Entity<VerificationCode>()
            .Property(code => code.Code)
            .HasMaxLength(255);
    }
}
