using Moq;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;

namespace CommonTestUtilities.Security;

public class IPasswordHasherBuilder
{
    private readonly Mock<IPasswordHasher> _mock;

    public IPasswordHasherBuilder()
    {
        _mock = new Mock<IPasswordHasher>();

        _mock.Setup(options => options.Ha(It.IsAny<string>()).Re;
    }

    public void ExistActiveUserWithEmail(string email)
    {
        _mock.Setup(options => options.ExisteActiveUserWithEmail(email)).ReturnsAsync(true);
    }

    public IPasswordHasher Build()
    {
        return _mock.Object;
    }
}
