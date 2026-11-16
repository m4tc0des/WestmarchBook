namespace WestmarchBook.Domain.Repositories.Reader;

public interface IReaderReadOnlyRepository
{
    Task<Entities.Reader?> GetById(long readerId);
    Task<bool> ExisteActiveReaderWithEmail(string email, long readId);
    Task<bool> ExisteActiveReaderWithDocumentNumber(string documentNumber, long readerId = 0);
    Task<IList<Entities.Reader>> GetAllActive();
    Task<long> GetLastReaderId();
}
