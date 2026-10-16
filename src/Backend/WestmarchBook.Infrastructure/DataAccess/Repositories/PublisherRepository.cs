using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Repositories.Publisher;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class PublisherRepository : IPublisherWriteOnlyRepository, IPublisherReadOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public PublisherRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Domain.Entities.Publisher publisher)
    {
        await _dbContext.Publishers.AddAsync(publisher);
    }

    public async Task<bool> ExistActivePublisherWithName(string name)
    {
        return await _dbContext.Publishers.AnyAsync(publisher => publisher.Active && publisher.Name.Equals(name));
    }
}
