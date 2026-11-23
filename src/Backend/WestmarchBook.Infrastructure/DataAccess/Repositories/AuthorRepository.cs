using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Author;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class AuthorRepository : IAuthorWriteOnlyRepository, IAuthorReadOnlyRepository, IAuthorUpdateOnlyRepository
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

    public void Update(Author author)
    {
        _dbContext.Authors.Update(author);
    }

    async Task<bool> IAuthorWriteOnlyRepository.DeleteById(long authorId)
    {
        var affected = await _dbContext
            .Authors
            .Where(author => author.Id == authorId)
            .ExecuteDeleteAsync();

        return affected > 0;
    }

    public async Task<bool> ExistActiveAuthorWithName(string name)
    {
        return await _dbContext.Authors.AnyAsync(author => author.Active && author.Name.Equals(name));
    }

    public async Task<bool> ExistActiveAuthorWithName(string name, long excludeAuthorId)
    {
        return await _dbContext.Authors.AnyAsync(author =>
            author.Active &&
            author.Name.Equals(name) &&
            author.Id != excludeAuthorId);
    }

    public async Task<bool> ExistActiveAuthorsWithIds(IList<long> authorIds)
    {
        var distinctIds = authorIds.Distinct().ToList();
        if (distinctIds.Count == 0) return false;

        var existingCount = await _dbContext.Authors
            .CountAsync(author => author.Active && distinctIds.Contains(author.Id));

        return existingCount == distinctIds.Count;
    }

    public async Task<bool> HasBooksLinked(long authorId)
    {
        return await _dbContext.BookAuthors.AnyAsync(ba => ba.AuthorId == authorId);
    }

    async Task<Author?> IAuthorReadOnlyRepository.GetById(long authorId)
    {
        return await _dbContext
            .Authors
            .AsNoTracking()
            .FirstOrDefaultAsync(author => author.Active && author.Id == authorId);
    }

    async Task<Author?> IAuthorUpdateOnlyRepository.GetById(long authorId)
    {
        return await _dbContext
            .Authors
            .FirstOrDefaultAsync(author => author.Active && author.Id == authorId);
    }

    public async Task<IList<Author>> GetAllActive()
    {
        return await _dbContext
            .Authors
            .AsNoTracking()
            .Where(author => author.Active)
            .OrderBy(author => author.Name)
            .ToListAsync();
    }
}
