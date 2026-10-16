namespace WestmarchBook.Domain.Repositories.Publisher;

public interface IPublisherReadOnlyRepository
{
    Task<bool> ExistActivePublisherWithName(string name);
}
