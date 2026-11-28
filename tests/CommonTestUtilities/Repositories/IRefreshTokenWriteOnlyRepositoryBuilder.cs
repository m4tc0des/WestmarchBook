using Moq;
using WestmarchBook.Domain.Repositories.RefreshToken;

namespace CommonTestUtilities.Repositories;

public class IRefreshTokenWriteOnlyRepositoryBuilder
{
    private readonly Mock<IRefreshTokenWriteOnlyRepository> _mock = new();

    public void VerifyDeleteByTokenCalled(string refreshToken, int times)
    {
        _mock.Verify(repo => repo.DeleteByToken(refreshToken), Times.Exactly(times));
    }

    public IRefreshTokenWriteOnlyRepository BuildInstance() => _mock.Object;

    public static IRefreshTokenWriteOnlyRepository Build()
    {
        return new Mock<IRefreshTokenWriteOnlyRepository>().Object;
    }
}
