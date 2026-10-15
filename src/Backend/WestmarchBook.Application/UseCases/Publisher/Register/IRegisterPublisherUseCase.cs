using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Publisher.Register;

public interface IRegisterPublisherUseCase
{
    Task<ResponseRegisterPublisherJson> Execute(RequestRegisterPublisherJson request);
}
