using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.GetById;

public class GetBookByIdUseCase : IGetBookByIdUseCase
{
    private readonly IBookReadOnlyRepository _bookReadRepository;
    private readonly ILoggedUser _loggedUser;

    public GetBookByIdUseCase(IBookReadOnlyRepository bookReadRepository, ILoggedUser loggedUser)
    {
        _bookReadRepository = bookReadRepository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseBookJson> Execute(long id)
    {
        var book = await _bookReadRepository.GetById(id, _loggedUser.GetUserId());

        if (book is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
        }

        return book.Adapt<ResponseBookJson>();
    }
}
