using WestmarchBook.Application.Extensions;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCase: IChangeProfilePictureUseCase
{
    private readonly ILoggedUser _loggedUser;

    public ChangeProfilePictureUseCase(ILoggedUser loggedUser)
    {
        _loggedUser = loggedUser;
    }

    public async Task Execute(Stream profilePicture)
    {
        var contentType = profilePicture.DetectImageContentType();

        if (contentType is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);
        }
    }
}
