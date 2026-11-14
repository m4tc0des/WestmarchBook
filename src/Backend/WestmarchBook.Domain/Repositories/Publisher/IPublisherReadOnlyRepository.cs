namespace WestmarchBook.Domain.Repositories.Publisher;

public interface IPublisherReadOnlyRepository
{
    Task<bool> ExistActivePublisherWithName(string name);
    Task<bool> ExistActivePublisherWithId(long publisherId);
    Task<IList<Entities.Publisher>> GetAllActive();
}
