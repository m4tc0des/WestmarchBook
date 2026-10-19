using Moq;
using WestmarchBook.Domain.Repositories.Author;

namespace CommonTestUtilities.Repositories;

public class IAuthorReadOnlyRepositoryBuilder
{
    private readonly Mock<IAuthorReadOnlyRepository> _mock;

    public IAuthorReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IAuthorReadOnlyRepository>();
    }

    public void ExistActiveAuthorWithName(string name)
    {
        _mock.Setup(options => options.ExistActiveAuthorWithName(name)).ReturnsAsync(true);
    }

    public IAuthorReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
