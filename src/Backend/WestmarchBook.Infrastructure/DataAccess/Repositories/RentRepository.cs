using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories.Rent;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class RentRepository : IRentWriteOnlyRepository, IRentReadOnlyRepository, IRentUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;
    public RentRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRent(Rent rent)
    {
        await _dbContext.AddAsync(rent);
    }

    public async Task<IList<Rent>> GetAll()
    {
        return await _dbContext
            .Rents
            .AsNoTracking()
            .Include(r => r.Reader)
            .Include(r => r.BookCopy)
                .ThenInclude(bc => bc.Book)
            .OrderByDescending(r => r.Id)
            .ToListAsync();
    }

    async Task<Rent?> IRentReadOnlyRepository.GetById(long rentId)
    {
        return await _dbContext
            .Rents
            .AsNoTracking()
            .Include(r => r.Reader)
            .Include(r => r.BookCopy)
                .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(r => r.Id == rentId);
    }

    async Task<Rent?> IRentUpdateOnlyRepository.GetById(long rentId)
    {
        return await _dbContext
            .Rents
            .Include(r => r.Reader)
            .Include(r => r.BookCopy)
                .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(r => r.Id == rentId);
    }

    public async Task<bool> HasActiveRentsForReader(long readerId, long excludeRentId = 0)
    {
        return await _dbContext
            .Rents
            .AsNoTracking()
            .AnyAsync(r => r.ReaderId == readerId && r.Id != excludeRentId && r.RentStatus == RentStatus.Active);
    }

    public async Task<bool> HasOverdueActiveRentsForReader(long readerId, DateTime referenceDate, long excludeRentId = 0)
    {
        var dateLimit = referenceDate.Date;
        return await _dbContext
            .Rents
            .AsNoTracking()
            .AnyAsync(r => r.ReaderId == readerId && r.Id != excludeRentId && r.RentStatus == RentStatus.Active && r.ExpectedReturnDate < dateLimit);
    }

    public void Update(Rent rent)
    {
        _dbContext.Rents.Update(rent);
    }

    public async Task<bool> DeleteById(long rentId)
    {
        var affected = await _dbContext
            .Rents
            .Where(r => r.Id == rentId)
            .ExecuteDeleteAsync();

        return affected > 0;
    }
}
