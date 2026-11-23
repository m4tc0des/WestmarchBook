using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.DeleteIllustration;

public class DeleteIllustrationUseCase : IDeleteIllustrationUseCase
{
    private readonly IBookUpdateOnlyRepository _bookUpdateRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIllustrationUseCase(
        IBookUpdateOnlyRepository bookUpdateRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _bookUpdateRepository = bookUpdateRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long bookId)
    {
        var book = await _bookUpdateRepository.GetById(bookId);

        if (book is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
        }

        if (book.HasImage)
        {
            await _storageService.DeleteBookIllustration(bookId);

            book.HasImage = false;

            await _unitOfWork.Commit();
        }
    }
}
