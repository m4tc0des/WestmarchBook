using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Dtos;
using WestmarchBook.Domain.Repositories.Book;

namespace WestmarchBook.Application.UseCases.Book.Filter;

public class FilterBookUseCase : IFilterBookUseCase
{
    private readonly IBookReadOnlyRepository _bookReadRepository;

    public FilterBookUseCase(IBookReadOnlyRepository bookReadRepository)
    {
        _bookReadRepository = bookReadRepository;
    }

    public async Task<ResponseBooksJson> Execute(RequestFilterBookJson? request)
    {
        var filter = request is null ? new BookFilterDto() :
            new BookFilterDto
            {
                SearchTerm = request.SearchTerm,
                Genre = (Domain.Enums.Genre?)request.Genre,
                PublisherId = request.PublisherId,
                AuthorId = request.AuthorId
            };

        var books = await _bookReadRepository.FilterBooks(filter);

        return new ResponseBooksJson
        {
            Books = books.Adapt<IList<ResponseBookSummaryJson>>()
        };
    }
}
