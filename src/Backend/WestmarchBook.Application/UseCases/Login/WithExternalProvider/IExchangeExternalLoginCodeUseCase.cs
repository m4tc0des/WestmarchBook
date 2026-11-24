using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Login.WithExternalProvider;

public interface IExchangeExternalLoginCodeUseCase
{
    Task<ResponseRegisterUserJson> Execute(RequestExternalLoginJson request);
}
