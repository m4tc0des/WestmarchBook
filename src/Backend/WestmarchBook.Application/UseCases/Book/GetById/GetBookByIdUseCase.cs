using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.GetById;

public class GetBookByIdUseCase : IGetBookByIdUseCase
{
    private readonly IBookReadOnlyRepository _bookReadRepository;

    public GetBookByIdUseCase(IBookReadOnlyRepository bookReadRepository)
    {
        _bookReadRepository = bookReadRepository;
    }

    public async Task<ResponseBookJson> Execute(long id)
    {
        var book = await _bookReadRepository.GetById(id);

        if (book is null) throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);

        return book.Adapt<ResponseBookJson>();
    }
}
