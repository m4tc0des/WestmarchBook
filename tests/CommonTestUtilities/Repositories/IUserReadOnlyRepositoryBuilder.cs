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

    public IUserReadOnlyRepositoryBuilder ExistActiveUserWithEmail(string email)
    {
        _mock.Setup(options => options.ExisteActiveUserWithEmail(email)).ReturnsAsync(true);

        return this;
    }

    public IUserReadOnlyRepositoryBuilder GetByEmail(User user)
    {
        _mock.Setup(options => options.GetByEmail(user.Email)).ReturnsAsync(user);

        return this;
    }

    public IUserReadOnlyRepositoryBuilder GetById(User user)
    {
        _mock.Setup(options => options.GetById(user.Id)).ReturnsAsync(user);

        return this;
    }

    public IUserReadOnlyRepositoryBuilder GetByIdForDeletion(User user)
    {
        _mock.Setup(options => options.GetByIdForDeletion(user.Id)).ReturnsAsync(user);

        return this;
    }

    public IUserReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
