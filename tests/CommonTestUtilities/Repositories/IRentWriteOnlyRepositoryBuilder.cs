using Moq;
using WestmarchBook.Domain.Repositories.Rent;

namespace CommonTestUtilities.Repositories;

public class IRentWriteOnlyRepositoryBuilder
{
    private readonly Mock<IRentWriteOnlyRepository> _mock;

    public IRentWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRentWriteOnlyRepository>();
    }

    public IRentWriteOnlyRepository Build()
    {
        return _mock.Object;
    }
}
