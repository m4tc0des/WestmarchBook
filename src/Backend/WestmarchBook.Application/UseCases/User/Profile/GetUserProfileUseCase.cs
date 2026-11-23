using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Services.StorageService;

namespace WestmarchBook.Application.UseCases.User.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IStorageService _storageService;
    public GetUserProfileUseCase(ILoggedUser loggedUser, IStorageService storageService)
    {
        _loggedUser = loggedUser;
        _storageService = storageService;
    }

    public async Task<ResponseUserProfileJson> Execute()
    {
        var loggedUser = await _loggedUser.GetProfile();

        return new ResponseUserProfileJson
        {
            UserName = loggedUser.UserName,
            Email = loggedUser.Email,
            ImageUrl = loggedUser.HasImage ? _storageService.GetProfilePictureUrl(loggedUser.Id) : string.Empty
        };
    }
}
