using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.DeleteById;

public class DeleteBookByIdUseCase : IDeleteBookByIdUseCase
{
    private readonly IBookDeleteOnlyRepository _bookDeleteRepository;

    public DeleteBookByIdUseCase(IBookDeleteOnlyRepository bookDeleteRepository)
    {
        _bookDeleteRepository = bookDeleteRepository;
    }

    public async Task Execute(long id)
    {
        var delete = await _bookDeleteRepository.DeleteById(id);

        if (delete == false) throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
    }
}
