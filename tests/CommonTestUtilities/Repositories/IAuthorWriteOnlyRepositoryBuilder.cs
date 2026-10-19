using Moq;
using WestmarchBook.Domain.Repositories.Author;

namespace CommonTestUtilities.Repositories;

public class IAuthorWriteOnlyRepositoryBuilder
{
    public static IAuthorWriteOnlyRepository Build()
    {
        var mock = new Mock<IAuthorWriteOnlyRepository>();

        return mock.Object;
    }
}
