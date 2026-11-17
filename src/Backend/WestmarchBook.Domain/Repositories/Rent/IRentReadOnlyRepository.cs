namespace WestmarchBook.Domain.Repositories.Rent;

public interface IRentReadOnlyRepository
{
    Task<IList<Entities.Rent>> GetAll();
    Task<Entities.Rent?> GetById(long rentId);
    Task<bool> HasActiveRentsForReader(long readerId, long excludeRentId = 0);
    Task<bool> HasOverdueActiveRentsForReader(long readerId, DateTime referenceDate, long excludeRentId = 0);
}
