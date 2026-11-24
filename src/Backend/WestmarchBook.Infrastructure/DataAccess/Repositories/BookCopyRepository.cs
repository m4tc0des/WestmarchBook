using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class BookCopyRepository : IBookCopyUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public BookCopyRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookCopy?> GetById(long id)
    {
        return await _dbContext.BookCopies.FirstOrDefaultAsync(copy => copy.Id == id);
    }

    public void Update(BookCopy bookCopy)
    {
        _dbContext.BookCopies.Update(bookCopy);
    }
}
