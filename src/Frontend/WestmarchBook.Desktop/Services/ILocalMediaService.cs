namespace WestmarchBook.Desktop.Services;

public interface ILocalMediaService
{
    string GetBookIllustrationPath(long bookId);
    string GetUserProfilePicturePath(long userId);
    string GetAuthorIllustrationPath(long authorId);
    string? GetExistingBookIllustrationPath(long bookId);
    string? GetExistingUserProfilePicturePath(long userId);
    string? GetExistingAuthorIllustrationPath(long authorId);
    Task<string> SaveBookIllustrationAsync(long bookId, Stream imageStream);
    Task<string> SaveUserProfilePictureAsync(long userId, Stream imageStream);
    Task<string> SaveAuthorIllustrationAsync(long authorId, Stream imageStream);
    Task<string> SavePreviewImageAsync(Stream imageStream);
    void DeleteImage(string? filePath);
    void DeleteBookIllustration(long bookId);
    void DeleteUserProfilePicture(long userId);
    void DeleteAuthorIllustration(long authorId);
}
