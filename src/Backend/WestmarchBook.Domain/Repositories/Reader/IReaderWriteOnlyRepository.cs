namespace WestmarchBook.Domain.Repositories.Reader;

public interface IReaderWriteOnlyRepository
{
    Task Add(Entities.Reader reader);
}
