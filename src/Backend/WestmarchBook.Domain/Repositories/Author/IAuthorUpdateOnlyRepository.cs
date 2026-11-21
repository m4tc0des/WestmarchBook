namespace WestmarchBook.Domain.Repositories.Author;

public interface IAuthorUpdateOnlyRepository
{
    Task<Entities.Author?> GetById(long authorId);
    void Update(Entities.Author author);
}
