using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Readers.Register;

public interface IRegisterReaderAccountUseCase
{
    Task<ResponseRegisterReaderJson> Execute(RequestRegisterReaderJson request);
}
