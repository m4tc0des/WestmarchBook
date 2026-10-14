using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Book.Register;

public interface IRegisterBookUseCase
{
    Task<ResponseRegisterBookJson> Execute(RequestRegisterBookJson request);
}
