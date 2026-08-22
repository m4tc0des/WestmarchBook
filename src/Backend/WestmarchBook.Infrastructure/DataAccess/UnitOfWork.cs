using WestmarchBook.Domain.Repositories;

namespace WestmarchBook.Infrastructure.DataAccess;

internal class UnitOfWork: IUnitOfWork
{
    private readonly WestmarchBookDbContext _dbContext;

    public UnitOfWork(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;    
    }

    public async Task Commit()
    {
        await _dbContext.SaveChangesAsync();
    }
}
