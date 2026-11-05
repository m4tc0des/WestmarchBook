using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Rent.Register;

public interface IRegisterRentUseCase
{
    Task<ResponseRegisterRentJson> Execute(RequestRegisterRentJson request);
}
