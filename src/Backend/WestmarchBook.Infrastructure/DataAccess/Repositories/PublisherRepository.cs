using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Publisher;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class PublisherRepository : IPublisherWriteOnlyRepository, IPublisherReadOnlyRepository, IPublisherUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public PublisherRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Publisher publisher)
    {
        await _dbContext.Publishers.AddAsync(publisher);
    }

    public void Update(Publisher publisher)
    {
        _dbContext.Publishers.Update(publisher);
    }

    async Task<bool> IPublisherWriteOnlyRepository.DeleteById(long publisherId)
    {
        var affected = await _dbContext
            .Publishers
            .Where(publisher => publisher.Id == publisherId)
            .ExecuteDeleteAsync();

        return affected > 0;
    }

    public async Task<bool> ExistActivePublisherWithName(string name)
    {
        return await _dbContext.Publishers.AnyAsync(publisher => publisher.Active && publisher.Name.Equals(name));
    }

    public async Task<bool> ExistActivePublisherWithName(string name, long excludePublisherId)
    {
        return await _dbContext.Publishers.AnyAsync(publisher =>
            publisher.Active &&
            publisher.Name.Equals(name) &&
            publisher.Id != excludePublisherId);
    }

    public async Task<bool> ExistActivePublisherWithId(long publisherId)
    {
        return await _dbContext.Publishers.AnyAsync(publisher => publisher.Active && publisher.Id == publisherId);
    }

    public async Task<bool> HasBooksLinked(long publisherId)
    {
        return await _dbContext.Books.AnyAsync(book => book.PublisherId == publisherId);
    }

    async Task<Publisher?> IPublisherReadOnlyRepository.GetById(long publisherId)
    {
        return await _dbContext
            .Publishers
            .AsNoTracking()
            .FirstOrDefaultAsync(publisher => publisher.Active && publisher.Id == publisherId);
    }

    async Task<Publisher?> IPublisherUpdateOnlyRepository.GetById(long publisherId)
    {
        return await _dbContext
            .Publishers
            .FirstOrDefaultAsync(publisher => publisher.Active && publisher.Id == publisherId);
    }

    public async Task<IList<Publisher>> GetAllActive()
    {
        return await _dbContext
            .Publishers
            .AsNoTracking()
            .Where(publisher => publisher.Active)
            .OrderBy(publisher => publisher.Name)
            .ToListAsync();
    }
}
