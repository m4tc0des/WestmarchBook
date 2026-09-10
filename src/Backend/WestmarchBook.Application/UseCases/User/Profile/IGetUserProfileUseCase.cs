using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.User.Profile;

public interface IGetUserProfileUseCase
{
    Task<ResponseUserProfileJson> Execute();
}
