using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Identity;

namespace CommonTestUtilities.Identity;

public class ILoggedUserBuilder
{
    public static ILoggedUser Build(User user)
    {
        var mock = new Mock<ILoggedUser>();

        mock.Setup(x => x.GetProfile()).ReturnsAsync(user);
        mock.Setup(x => x.GetUserId()).Returns(user.Id);

        return mock.Object;
    }
}
