using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;

public interface ILoginWithEmailAndPasswordUseCase
{
    Task<ResponseRegisterUserJson> Execute(RequestLoginJson request);
}
