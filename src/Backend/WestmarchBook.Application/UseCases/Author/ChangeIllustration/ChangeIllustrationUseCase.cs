using WestmarchBook.Application.Extensions;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.ChangeIllustration;

public class ChangeIllustrationUseCase : IChangeIllustrationUseCase
{
    private readonly IAuthorUpdateOnlyRepository _authorUpdateRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeIllustrationUseCase(
        IAuthorUpdateOnlyRepository authorUpdateRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _authorUpdateRepository = authorUpdateRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long authorId, Stream authorIllustration)
    {
        var contentType = authorIllustration.DetectImageContentType();

        if (contentType is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);
        }

        var author = await _authorUpdateRepository.GetById(authorId);

        if (author is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        await _storageService.UploadAuthorIllustration(author, authorIllustration, contentType);

        author.HasImage = true;
        _authorUpdateRepository.Update(author);

        await _unitOfWork.Commit();
    }
}
