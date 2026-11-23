using Microsoft.Maui.Storage;

namespace WestmarchBook.Desktop.Services;

public sealed class LocalMediaService : ILocalMediaService
{
    private readonly string _mediaDirectory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LocalMediaService()
        : this(Path.Combine(FileSystem.AppDataDirectory, "media"))
    {
    }

    public LocalMediaService(string mediaDirectory)
    {
        _mediaDirectory = mediaDirectory;
    }

    public string GetBookIllustrationPath(long bookId)
    {
        return Path.Combine(_mediaDirectory, $"book_{bookId}.jpg");
    }

    public string GetUserProfilePicturePath(long userId)
    {
        return Path.Combine(_mediaDirectory, $"user_{userId}.jpg");
    }

    public string GetAuthorIllustrationPath(long authorId)
    {
        return Path.Combine(_mediaDirectory, $"author_{authorId}.jpg");
    }

    public string? GetExistingBookIllustrationPath(long bookId)
    {
        var path = GetBookIllustrationPath(bookId);
        return File.Exists(path) ? path : null;
    }

    public string? GetExistingUserProfilePicturePath(long userId)
    {
        var path = GetUserProfilePicturePath(userId);
        return File.Exists(path) ? path : null;
    }

    public string? GetExistingAuthorIllustrationPath(long authorId)
    {
        var path = GetAuthorIllustrationPath(authorId);
        return File.Exists(path) ? path : null;
    }

    public Task<string> SaveBookIllustrationAsync(long bookId, Stream imageStream)
    {
        return SaveFileAsync(GetBookIllustrationPath(bookId), imageStream);
    }

    public Task<string> SaveUserProfilePictureAsync(long userId, Stream imageStream)
    {
        return SaveFileAsync(GetUserProfilePicturePath(userId), imageStream);
    }

    public Task<string> SaveAuthorIllustrationAsync(long authorId, Stream imageStream)
    {
        return SaveFileAsync(GetAuthorIllustrationPath(authorId), imageStream);
    }

    public Task<string> SavePreviewImageAsync(Stream imageStream)
    {
        var previewPath = Path.Combine(_mediaDirectory, $"preview_{Guid.NewGuid():N}.jpg");
        return SaveFileAsync(previewPath, imageStream);
    }

    public void DeleteImage(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        _semaphore.Wait();
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void DeleteBookIllustration(long bookId)
    {
        DeleteImage(GetBookIllustrationPath(bookId));
    }

    public void DeleteUserProfilePicture(long userId)
    {
        DeleteImage(GetUserProfilePicturePath(userId));
    }

    public void DeleteAuthorIllustration(long authorId)
    {
        DeleteImage(GetAuthorIllustrationPath(authorId));
    }

    private async Task<string> SaveFileAsync(string destinationPath, Stream imageStream)
    {
        ArgumentNullException.ThrowIfNull(imageStream);

        await _semaphore.WaitAsync();
        try
        {
            Directory.CreateDirectory(_mediaDirectory);

            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            await using var fileStream = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await imageStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            return destinationPath;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
