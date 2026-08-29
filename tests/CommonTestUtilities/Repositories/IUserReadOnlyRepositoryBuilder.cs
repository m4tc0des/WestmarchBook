using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class IUserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _mock;

    public IUserReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IUserReadOnlyRepository>();
    }

    public void ExistActiveUserWithEmail(string email)
    {
        _mock.Setup(options => options.ExisteActiveUserWithEmail(email)).ReturnsAsync(true);
    }

    public void GetByEmail(User user)
    {
        _mock.Setup(options => options.GetByEmail(user.Email)).ReturnsAsync(user);
    }

    public IUserReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
