using Moq;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookWriteOnlyRepositoryBuilder
{
    public static IBookWriteOnlyRepository Build()
    {
        var mock = new Mock<IBookWriteOnlyRepository>();

        return mock.Object;
    }
}
