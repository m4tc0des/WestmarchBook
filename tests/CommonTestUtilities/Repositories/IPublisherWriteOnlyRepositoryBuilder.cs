using Moq;
using WestmarchBook.Domain.Repositories.Publisher;

namespace CommonTestUtilities.Repositories;

public class IPublisherWriteOnlyRepositoryBuilder
{
    public static IPublisherWriteOnlyRepository Build()
    {
        var mock = new Mock<IPublisherWriteOnlyRepository>();

        return mock.Object;
    }
}
