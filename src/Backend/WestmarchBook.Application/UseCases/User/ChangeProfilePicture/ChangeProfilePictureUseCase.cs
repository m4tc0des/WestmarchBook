using WestmarchBook.Application.Extensions;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCase: IChangeProfilePictureUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IStorageService _storageService;

    public ChangeProfilePictureUseCase(ILoggedUser loggedUser, IStorageService storageService)
    {
        _loggedUser = loggedUser;
        _storageService = storageService;
    }

    public async Task Execute(Stream profilePicture)
    {
        var contentType = profilePicture.DetectImageContentType();

        if (contentType is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);
        }
        var loggedUser = await _loggedUser.GetProfile();

        await _storageService.UploadProfilePicture(loggedUser, profilePicture, contentType);
    }
}
