using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Author.Register;

public interface IRegisterAuthorUseCase
{
    Task<ResponseRegisterAuthorJson> Execute(RequestRegisterAuthorJson request);
}
