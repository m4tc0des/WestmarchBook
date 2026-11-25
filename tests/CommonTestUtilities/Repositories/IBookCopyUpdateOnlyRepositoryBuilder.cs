using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace CommonTestUtilities.Repositories;

public class IBookCopyUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IBookCopyUpdateOnlyRepository> _mock;

    public IBookCopyUpdateOnlyRepositoryBuilder()
    {
        _mock = new Mock<IBookCopyUpdateOnlyRepository>();
    }

    public IBookCopyUpdateOnlyRepositoryBuilder GetById(BookCopy? bookCopy)
    {
        if (bookCopy is not null)
        {
            _mock.Setup(repository => repository.GetById(bookCopy.Id)).ReturnsAsync(bookCopy);
        }

        return this;
    }

    public void VerifyUpdateCalled(int times = 1)
    {
        _mock.Verify(repository => repository.Update(It.IsAny<BookCopy>()), Times.Exactly(times));
    }

    public IBookCopyUpdateOnlyRepository Build()
    {
        return _mock.Object;
    }
}
