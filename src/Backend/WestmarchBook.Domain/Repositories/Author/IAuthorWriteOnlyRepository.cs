namespace WestmarchBook.Domain.Repositories.Author;

public interface IAuthorWriteOnlyRepository
{
    Task Add(Entities.Author author);
    void Update(Entities.Author author);
    Task<bool> DeleteById(long authorId);
}
