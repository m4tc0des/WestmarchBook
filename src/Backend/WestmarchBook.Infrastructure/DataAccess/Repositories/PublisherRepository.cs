using WestmarchBook.Domain.Repositories.Publisher;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class PublisherRepository: IPublisherWriteOnlyRepository
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
}
