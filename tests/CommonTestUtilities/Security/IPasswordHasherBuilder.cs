using Moq;
using WestmarchBook.Domain.Security.PasswordHashing;

namespace CommonTestUtilities.Security;

public class IPasswordHasherBuilder
{
    private readonly Mock<IPasswordHasher> _mock;

    public IPasswordHasherBuilder()
    {
        _mock = new Mock<IPasswordHasher>();

        _mock.Setup(x => x.PasswordHash(It.IsAny<string>())).Returns("hashed_password");
    }

    public void ExistActiveUserWithEmail(string password)
    {
        _mock.Setup(repo => repo.VerifyPassword(password, It.IsAny<string>())).Returns(true);
    }

    public IPasswordHasher Build()
    {
        return _mock.Object;
    }
}
