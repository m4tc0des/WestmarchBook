using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.ActiveById;

public class ActiveBookByIdUseCase : IActiveBookByIdUseCase
{
    private readonly IBookWriteOnlyRepository _bookReadRepository;

    public ActiveBookByIdUseCase(IBookWriteOnlyRepository bookReadRepository)
    {
        _bookReadRepository = bookReadRepository;
    }

    public async Task Execute(long id)
    {
        var active = await _bookReadRepository.ActiveById(id);

        if (active == false) throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
    }
}
