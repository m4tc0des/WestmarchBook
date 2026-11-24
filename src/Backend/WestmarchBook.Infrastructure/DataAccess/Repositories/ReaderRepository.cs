using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Reader;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class ReaderRepository : IReaderWriteOnlyRepository, IReaderReadOnlyRepository, IReaderUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public ReaderRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Reader reader)
    {
        await _dbContext.Readers.AddAsync(reader);
    }

    public async Task<bool> ExisteActiveReaderWithEmail(string email, long readerId)
    {
        return await _dbContext.Readers.AnyAsync(reader => reader.Active && reader.Email.Equals(email) && reader.Id != readerId);
    }

    public async Task<bool> ExisteActiveReaderWithDocumentNumber(string documentNumber, long readerId = 0)
    {
        return await _dbContext.Readers.AnyAsync(reader => reader.Active && reader.DocumentNumber.Equals(documentNumber) && reader.Id != readerId);
    }

    public async Task<Reader?> GetById(long readerId)
    {
        return await _dbContext
            .Readers
            .FirstOrDefaultAsync(reader => reader.Active && reader.Id == readerId);
    }

    public async Task<IList<Reader>> GetAllActive()
    {
        return await _dbContext
            .Readers
            .AsNoTracking()
            .Where(reader => reader.Active)
            .OrderBy(reader => reader.Id)
            .ToListAsync();
    }

    public async Task<long> GetLastReaderId()
    {
        return await _dbContext
            .Readers
            .AsNoTracking()
            .Select(reader => (long?)reader.Id)
            .MaxAsync() ?? 0;
    }

    public void UpdateReader(Reader reader)
    {
        _dbContext.Readers.Update(reader);
    }
}
