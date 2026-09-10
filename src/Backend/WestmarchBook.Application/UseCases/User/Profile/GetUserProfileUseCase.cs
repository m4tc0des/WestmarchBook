using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;

namespace WestmarchBook.Application.UseCases.User.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    public GetUserProfileUseCase(ILoggedUser loggedUser)
    {
        _loggedUser = loggedUser;
    }

    public async Task<ResponseUserProfileJson> Execute()
    {
        var loggedUser = await _loggedUser.GetProfile();

        return loggedUser.Adapt<ResponseUserProfileJson>();
    }
}
