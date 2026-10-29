using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Book;

namespace WestmarchBook.Application.UseCases.Book.GetAll;

public class GetAllBooksUseCase : IGetAllBooksUseCase
{
    private readonly IBookReadOnlyRepository _bookReadRepository;

    public GetAllBooksUseCase(IBookReadOnlyRepository bookReadRepository)
    {
        _bookReadRepository = bookReadRepository;
    }

    public async Task<IList<ResponseBookJson>> Execute()
    {
        var books = await _bookReadRepository.GetAllActive();

        return books.Adapt<IList<ResponseBookJson>>();
    }

    public async Task<long> GetNextBookId()
    {
        var lastId = await _bookReadRepository.GetLastBookId();

        return lastId + 1;
    }
}
