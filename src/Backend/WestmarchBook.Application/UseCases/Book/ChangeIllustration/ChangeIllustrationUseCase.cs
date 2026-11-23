using WestmarchBook.Application.Extensions;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.ChangeIllustration;

public class ChangeIllustrationUseCase : IChangeIllustrationUseCase
{
    private readonly IBookUpdateOnlyRepository _bookUpdateRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeIllustrationUseCase(
        IBookUpdateOnlyRepository bookUpdateRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _bookUpdateRepository = bookUpdateRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long bookId, Stream bookIllustration)
    {
        var contentType = bookIllustration.DetectImageContentType();

        if (contentType is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);
        }

        var book = await _bookUpdateRepository.GetById(bookId);

        if (book is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
        }

        await _storageService.UploadIllustration(book, bookIllustration, contentType);

        book.HasImage = true;

        await _unitOfWork.Commit();
    }
}
