namespace WestmarchBook.Domain.Repositories.Author;

public interface IAuthorReadOnlyRepository
{
    Task<bool> ExistActiveAuthorWithName(string name);
    Task<bool> ExistActiveAuthorWithName(string name, long excludeAuthorId);
    Task<bool> ExistActiveAuthorsWithIds(IList<long> authorIds);
    Task<bool> HasBooksLinked(long authorId);
    Task<Entities.Author?> GetById(long authorId);
    Task<IList<Entities.Author>> GetAllActive();
}
