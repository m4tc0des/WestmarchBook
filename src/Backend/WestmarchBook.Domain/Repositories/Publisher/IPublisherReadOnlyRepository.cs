namespace WestmarchBook.Domain.Repositories.Publisher;

public interface IPublisherReadOnlyRepository
{
    Task<bool> ExistActivePublisherWithName(string name);
    Task<bool> ExistActivePublisherWithName(string name, long excludePublisherId);
    Task<bool> ExistActivePublisherWithId(long publisherId);
    Task<bool> HasBooksLinked(long publisherId);
    Task<Entities.Publisher?> GetById(long publisherId);
    Task<IList<Entities.Publisher>> GetAllActive();
}
