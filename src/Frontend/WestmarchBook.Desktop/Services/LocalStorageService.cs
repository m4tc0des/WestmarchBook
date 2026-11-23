using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Services.StorageService;

namespace WestmarchBook.Desktop.Services;

public sealed class LocalStorageService : IStorageService
{
    private readonly ILocalMediaService _localMediaService;

    public LocalStorageService(ILocalMediaService localMediaService)
    {
        _localMediaService = localMediaService;
    }

    public async Task UploadProfilePicture(User user, Stream file, string contentType)
    {
        await _localMediaService.SaveUserProfilePictureAsync(user.Id, file);
    }

    public async Task UploadIllustration(Book book, Stream file, string contentType)
    {
        await _localMediaService.SaveBookIllustrationAsync(book.Id, file);
    }

    public async Task UploadAuthorIllustration(Author author, Stream file, string contentType)
    {
        await _localMediaService.SaveAuthorIllustrationAsync(author.Id, file);
    }

    public string GetProfilePictureUrl(long userId)
    {
        return _localMediaService.GetExistingUserProfilePicturePath(userId) ?? string.Empty;
    }

    public string GetBookIllustrationUrl(long bookId)
    {
        return _localMediaService.GetExistingBookIllustrationPath(bookId) ?? string.Empty;
    }

    public string GetAuthorIllustrationUrl(long authorId)
    {
        return _localMediaService.GetExistingAuthorIllustrationPath(authorId) ?? string.Empty;
    }

    public Task DeleteUserFiles(User user)
    {
        _localMediaService.DeleteUserProfilePicture(user.Id);
        return Task.CompletedTask;
    }

    public Task DeleteBookIllustration(long bookId)
    {
        _localMediaService.DeleteBookIllustration(bookId);
        return Task.CompletedTask;
    }

    public Task DeleteAuthorIllustration(long authorId)
    {
        _localMediaService.DeleteAuthorIllustration(authorId);
        return Task.CompletedTask;
    }
}
