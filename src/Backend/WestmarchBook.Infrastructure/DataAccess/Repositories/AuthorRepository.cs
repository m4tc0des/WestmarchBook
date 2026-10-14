using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Author;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class AuthorRepository: IAuthorWriteOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public AuthorRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Author author)
    {
        await _dbContext.Authors.AddAsync(author);
    }
}
