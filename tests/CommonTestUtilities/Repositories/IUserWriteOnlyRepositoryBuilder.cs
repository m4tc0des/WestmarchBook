using Moq;
using WestmarchBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class IUserWriteOnlyRepositoryBuilder
{
    private readonly Mock<IUserWriteOnlyRepository> _mock = new();

    public IUserWriteOnlyRepository BuildInstance() => _mock.Object;

    public void VerifyDeactivateAccountCalled(long userId, int times = 1)
    {
        _mock.Verify(repository => repository.DeactivateAccount(userId), Times.Exactly(times));
    }

    public void VerifyDeleteAccountCalled(long userId, int times = 1)
    {
        _mock.Verify(repository => repository.DeleteAccount(userId), Times.Exactly(times));
    }

    public static IUserWriteOnlyRepository Build()
    {
        var mock = new Mock<IUserWriteOnlyRepository>();

        return mock.Object;
    }
}
