using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Publisher;

namespace CommonTestUtilities.Repositories;

public class IPublisherUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IPublisherUpdateOnlyRepository> _mock = new();

    public IPublisherUpdateOnlyRepositoryBuilder GetById(Publisher? publisher)
    {
        if (publisher is not null)
        {
            _mock.Setup(repo => repo.GetById(publisher.Id)).ReturnsAsync(publisher);
        }

        return this;
    }

    public void VerifyUpdateCalled(int times = 1)
    {
        _mock.Verify(repo => repo.Update(It.IsAny<Publisher>()), Times.Exactly(times));
    }

    public IPublisherUpdateOnlyRepository Build() => _mock.Object;
}
