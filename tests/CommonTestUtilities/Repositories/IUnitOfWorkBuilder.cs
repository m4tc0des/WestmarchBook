using Moq;
using WestmarchBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class IUnitOfWorkBuilder
{
    private readonly Mock<IUnitOfWork> _mock = new();

    public IUnitOfWork BuildInstance() => _mock.Object;

    public void VerifyCommitCalled(int times = 1)
    {
        _mock.Verify(unitOfWork => unitOfWork.Commit(), Times.Exactly(times));
    }

    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();

        return mock.Object;
    }
}
