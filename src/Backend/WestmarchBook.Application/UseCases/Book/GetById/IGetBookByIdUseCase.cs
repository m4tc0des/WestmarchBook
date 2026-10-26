using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Book.GetById;

public interface IGetBookByIdUseCase
{
    Task<ResponseBookJson> Execute(long id);
}
