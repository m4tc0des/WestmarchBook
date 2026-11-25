using Moq;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookWriteOnlyRepositoryBuilder
{
    private readonly Mock<IBookWriteOnlyRepository> _mock;

    public IBookWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IBookWriteOnlyRepository>();
    }

    public IBookWriteOnlyRepositoryBuilder ActiveById(long bookId)
    {
        _mock.Setup(repository => repository.ActiveById(bookId)).ReturnsAsync(true);
        return this;
    }

    public IBookWriteOnlyRepository BuildInstance() => _mock.Object;

    public static IBookWriteOnlyRepository Build()
    {
        var mock = new Mock<IBookWriteOnlyRepository>();

        return mock.Object;
    }
}
