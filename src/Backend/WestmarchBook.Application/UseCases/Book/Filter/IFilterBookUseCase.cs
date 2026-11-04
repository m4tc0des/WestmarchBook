using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Book.Filter;

public interface IFilterBookUseCase
{
    Task<ResponseBooksJson> Execute(RequestFilterBookJson? request);
}
