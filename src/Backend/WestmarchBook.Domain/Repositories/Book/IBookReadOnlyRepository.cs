namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookReadOnlyRepository
{
    Task<bool> ExistActiveBookWithTitle(string title);
}
