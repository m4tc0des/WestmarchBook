using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Author.GetAll;

public interface IGetAllAuthorsUseCase
{
    Task<IList<ResponseAuthorJson>> Execute();
}
