using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class BookRepository: IBookWriteOnlyRepository, IBookReadOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public BookRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Book book)
    {
        await _dbContext.Books.AddAsync(book);
    }

    public async Task<bool> ExistActiveBookWithTitle(string title)
    {
        return await _dbContext.Books.AnyAsync(book => book.Active && book.Title.Equals(title));
    }
}
