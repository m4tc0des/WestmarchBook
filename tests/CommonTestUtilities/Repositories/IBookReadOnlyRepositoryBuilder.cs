using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookReadOnlyRepositoryBuilder
{
    private readonly Mock<IBookReadOnlyRepository> _mock;

    public IBookReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IBookReadOnlyRepository>();
    }

    public void ExistActiveBookWithTitle(string title)
    {
        _mock.Setup(options => options.ExistActiveBookWithTitle(title)).ReturnsAsync(true);
    }

    public IBookReadOnlyRepositoryBuilder GetById(Book book)
    {
        _mock.Setup(options => options.GetById(book.Id, book.UserId)).ReturnsAsync(book);

        return this;
    }

    public IBookReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
