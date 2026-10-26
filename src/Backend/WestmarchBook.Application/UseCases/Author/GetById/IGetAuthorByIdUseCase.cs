using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Author.GetById;

public interface IGetAuthorByIdUseCase
{
    Task<ResponseAuthorJson> Execute(long id);
}
