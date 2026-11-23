using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Domain.Services.StorageService;

public interface IStorageService
{
    Task UploadProfilePicture(User user, Stream file, string contentType);
    Task UploadIllustration(Book book, Stream file, string contentType);
    Task UploadAuthorIllustration(Author author, Stream file, string contentType);
    string GetProfilePictureUrl(long userId);
    string GetBookIllustrationUrl(long bookId);
    string GetAuthorIllustrationUrl(long authorId);
    Task DeleteUserFiles(User user);
    Task DeleteBookIllustration(long bookId);
    Task DeleteAuthorIllustration(long authorId);
}
