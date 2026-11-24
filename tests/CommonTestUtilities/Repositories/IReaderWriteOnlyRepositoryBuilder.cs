using Moq;
using WestmarchBook.Domain.Repositories.Reader;

namespace CommonTestUtilities.Repositories;

public class IReaderWriteOnlyRepositoryBuilder
{
    public static IReaderWriteOnlyRepository Build()
    {
        var mock = new Mock<IReaderWriteOnlyRepository>();

        return mock.Object;
    }
}
