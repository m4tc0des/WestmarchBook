using Moq;
using WestmarchBook.Domain.Repositories.Reader;

namespace CommonTestUtilities.Repositories;

public class IReaderUpdateOnlyRepositoryBuilder
{
    public static IReaderUpdateOnlyRepository Build()
    {
        var mock = new Mock<IReaderUpdateOnlyRepository>();

        return mock.Object;
    }
}
