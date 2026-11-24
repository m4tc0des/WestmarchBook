using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Rent;

namespace CommonTestUtilities.Repositories;

public class IRentUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IRentUpdateOnlyRepository> _mock;

    public IRentUpdateOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRentUpdateOnlyRepository>();
    }

    public IRentUpdateOnlyRepositoryBuilder GetById(Rent? rent)
    {
        if (rent is not null)
        {
            _mock.Setup(repository => repository.GetById(rent.Id)).ReturnsAsync(rent);
        }

        return this;
    }

    public void VerifyUpdateCalled(int times = 1)
    {
        _mock.Verify(repository => repository.Update(It.IsAny<Rent>()), Times.Exactly(times));
    }

    public IRentUpdateOnlyRepository Build()
    {
        return _mock.Object;
    }
}
