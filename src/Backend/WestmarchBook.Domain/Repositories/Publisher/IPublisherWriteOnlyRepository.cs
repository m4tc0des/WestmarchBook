namespace WestmarchBook.Domain.Repositories.Publisher;

public interface IPublisherWriteOnlyRepository
{
    Task Add(Entities.Publisher publisher);
}
