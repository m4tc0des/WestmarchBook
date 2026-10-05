using Moq;
using WestmarchBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class IUserUpdateOnlyRepositoryBuilder
{
    public static IUserUpdateOnlyRepository Build()
    {
        var mock = new Mock<IUserUpdateOnlyRepository>();

        return mock.Object;
    }
}
