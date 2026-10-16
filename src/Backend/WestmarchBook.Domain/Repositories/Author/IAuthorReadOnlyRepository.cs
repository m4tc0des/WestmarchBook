namespace WestmarchBook.Domain.Repositories.Author;

public interface IAuthorReadOnlyRepository
{
    Task<bool> ExistActiveAuthorWithName(string name);
}
