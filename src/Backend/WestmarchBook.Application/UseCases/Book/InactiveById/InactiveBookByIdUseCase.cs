using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.InactiveById;

public class InactiveBookByIdUseCase : IInactiveBookByIdUseCase
{
    private readonly IBookDeleteOnlyRepository _bookDeleteRepository;

    public InactiveBookByIdUseCase(IBookDeleteOnlyRepository bookDeleteRepository)
    {
        _bookDeleteRepository = bookDeleteRepository;
    }

    public async Task Execute(long id)
    {
        var inactive = await _bookDeleteRepository.InactiveById(id);

        if (inactive == false) throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);

    }
}
