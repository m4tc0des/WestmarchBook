using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Genre.Register;

public interface IRegisterGenreUseCase
{
    Task<ResponseRegisterGenreJson> Execute(RequestRegisterGenreJson request);
}
