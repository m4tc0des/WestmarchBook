using Moq;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookDeleteOnlyRepositoryBuilder
{
    private readonly Mock<IBookDeleteOnlyRepository> _mock;

    public IBookDeleteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IBookDeleteOnlyRepository>();
    }

    public IBookDeleteOnlyRepositoryBuilder InactiveById(long bookId)
    {
        _mock.Setup(repository => repository.InactiveById(bookId)).ReturnsAsync(true);
        return this;
    }

    public IBookDeleteOnlyRepositoryBuilder DeleteById(long bookId)
    {
        _mock.Setup(repository => repository.DeleteById(bookId)).ReturnsAsync(true);
        return this;
    }

    public IBookDeleteOnlyRepository Build() => _mock.Object;
}
