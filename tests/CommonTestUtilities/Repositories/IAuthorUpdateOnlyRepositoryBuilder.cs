using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Author;

namespace CommonTestUtilities.Repositories;

public class IAuthorUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IAuthorUpdateOnlyRepository> _mock = new();

    public IAuthorUpdateOnlyRepositoryBuilder GetById(Author? author)
    {
        if (author is not null)
        {
            _mock.Setup(repo => repo.GetById(author.Id)).ReturnsAsync(author);
        }

        return this;
    }

    public void VerifyUpdateCalled(int times = 1)
    {
        _mock.Verify(repo => repo.Update(It.IsAny<Author>()), Times.Exactly(times));
    }

    public IAuthorUpdateOnlyRepository Build() => _mock.Object;
}
