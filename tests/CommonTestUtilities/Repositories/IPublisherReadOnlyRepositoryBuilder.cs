using Moq;
using WestmarchBook.Domain.Repositories.Publisher;

namespace CommonTestUtilities.Repositories;

public class IPublisherReadOnlyRepositoryBuilder
{
    private readonly Mock<IPublisherReadOnlyRepository> _mock;

    public IPublisherReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IPublisherReadOnlyRepository>();
    }

    public void ExistActivePublisherWithName(string name)
    {
        _mock.Setup(options => options.ExistActivePublisherWithName(name)).ReturnsAsync(true);
    }

    public IPublisherReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
