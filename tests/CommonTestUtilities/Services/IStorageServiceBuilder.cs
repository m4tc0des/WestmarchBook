using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Services.StorageService;

namespace CommonTestUtilities.Services;

public class IStorageServiceBuilder
{
    private readonly Mock<IStorageService> _mock;

    public IStorageServiceBuilder()
    {
        _mock = CreateConfiguredMock();
    }

    public IStorageService BuildInstance() => _mock.Object;

    public void VerifyUploadIllustrationCalled(int times = 1)
    {
        _mock.Verify(service => service.UploadIllustration(
            It.IsAny<Book>(),
            It.IsAny<Stream>(),
            It.IsAny<string>()), Times.Exactly(times));
    }

    public void VerifyUploadProfilePictureCalled(int times = 1)
    {
        _mock.Verify(service => service.UploadProfilePicture(
            It.IsAny<User>(),
            It.IsAny<Stream>(),
            It.IsAny<string>()), Times.Exactly(times));
    }

    public void VerifyDeleteBookIllustrationCalled(int times = 1)
    {
        _mock.Verify(service => service.DeleteBookIllustration(It.IsAny<long>()), Times.Exactly(times));
    }

    public void VerifyUploadAuthorIllustrationCalled(int times = 1)
    {
        _mock.Verify(service => service.UploadAuthorIllustration(
            It.IsAny<Author>(),
            It.IsAny<Stream>(),
            It.IsAny<string>()), Times.Exactly(times));
    }

    public void VerifyDeleteAuthorIllustrationCalled(int times = 1)
    {
        _mock.Verify(service => service.DeleteAuthorIllustration(It.IsAny<long>()), Times.Exactly(times));
    }

    public void VerifyDeleteUserFilesCalled(int times = 1)
    {
        _mock.Verify(service => service.DeleteUserFiles(It.IsAny<User>()), Times.Exactly(times));
    }

    public void VerifyDeleteUserFilesCalled(User user, int times = 1)
    {
        _mock.Verify(service => service.DeleteUserFiles(user), Times.Exactly(times));
    }

    public static IStorageService Build()
    {
        return CreateConfiguredMock().Object;
    }

    private static Mock<IStorageService> CreateConfiguredMock()
    {
        var mock = new Mock<IStorageService>();

        mock.Setup(service => service.GetProfilePictureUrl(It.IsAny<long>()))
            .Returns((long userId) => $"https://storage.test/user-{userId}/profile-picture.png");

        mock.Setup(service => service.GetBookIllustrationUrl(It.IsAny<long>()))
            .Returns((long bookId) => $"https://storage.test/book-{bookId}/cover.png");

        mock.Setup(service => service.GetAuthorIllustrationUrl(It.IsAny<long>()))
            .Returns((long authorId) => $"https://storage.test/author-{authorId}/photo.png");

        mock.Setup(service => service.UploadProfilePicture(
                It.IsAny<User>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mock.Setup(service => service.UploadIllustration(
                It.IsAny<Book>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mock.Setup(service => service.UploadAuthorIllustration(
                It.IsAny<Author>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mock.Setup(service => service.DeleteBookIllustration(It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        mock.Setup(service => service.DeleteAuthorIllustration(It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        mock.Setup(service => service.DeleteUserFiles(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        return mock;
    }
}
