namespace WestmarchBook.Domain.Repositories.Publisher;

public interface IPublisherUpdateOnlyRepository
{
    Task<Entities.Publisher?> GetById(long publisherId);
    void Update(Entities.Publisher publisher);
}
