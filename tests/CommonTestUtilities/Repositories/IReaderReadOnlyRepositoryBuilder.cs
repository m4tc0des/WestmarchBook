using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Reader;

namespace CommonTestUtilities.Repositories;

public class IReaderReadOnlyRepositoryBuilder
{
    private readonly Mock<IReaderReadOnlyRepository> _mock;

    public IReaderReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IReaderReadOnlyRepository>();
    }

    public IReaderReadOnlyRepositoryBuilder GetById(Reader reader)
    {
        _mock.Setup(repository => repository.GetById(reader.Id)).ReturnsAsync(reader);

        return this;
    }

    public void ExisteActiveReaderWithEmail(string email)
    {
        _mock.Setup(repository => repository.ExisteActiveReaderWithEmail(email, It.IsAny<long>())).ReturnsAsync(true);
    }

    public void ExisteActiveReaderWithDocumentNumber(string documentNumber)
    {
        var digits = new string(documentNumber.Where(char.IsDigit).ToArray());
        _mock.Setup(repository => repository.ExisteActiveReaderWithDocumentNumber(digits, It.IsAny<long>())).ReturnsAsync(true);
    }

    public IReaderReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
