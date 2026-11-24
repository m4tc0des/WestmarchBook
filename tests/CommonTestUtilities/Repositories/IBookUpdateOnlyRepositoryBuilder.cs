using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IBookUpdateOnlyRepository> _mock;

    public IBookUpdateOnlyRepositoryBuilder()
    {
        _mock = new Mock<IBookUpdateOnlyRepository>();
    }

    public IBookUpdateOnlyRepositoryBuilder GetById(Book book)
    {
        _mock.Setup(options => options.GetById(book.Id)).ReturnsAsync(book);

        return this;
    }

    public IBookUpdateOnlyRepository Build()
    {
        return _mock.Object;
    }
}
