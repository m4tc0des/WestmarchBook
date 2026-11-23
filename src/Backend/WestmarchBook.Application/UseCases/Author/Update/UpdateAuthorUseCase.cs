using WestmarchBook.Application.Extensions;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.Update;

public class UpdateAuthorUseCase : IUpdateAuthorUseCase
{
    private readonly IAuthorUpdateOnlyRepository _authorUpdateRepository;
    private readonly IAuthorReadOnlyRepository _authorReadRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAuthorUseCase(
        IAuthorUpdateOnlyRepository authorUpdateRepository,
        IAuthorReadOnlyRepository authorReadRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _authorUpdateRepository = authorUpdateRepository;
        _authorReadRepository = authorReadRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long id, RequestUpdateAuthorJson request, Stream? authorIllustration = null)
    {
        var author = await _authorUpdateRepository.GetById(id);

        if (author is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        await ValidateAndThrowOnFailures(id, request);

        string? contentType = null;

        if (authorIllustration is not null)
        {
            contentType = authorIllustration.DetectImageContentType();

            if (contentType is null)
            {
                throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);
            }

            author.HasImage = true;
        }

        if (request.Name is not null)
        {
            author.Name = request.Name.Trim();
        }

        _authorUpdateRepository.Update(author);

        await _unitOfWork.Commit();

        if (authorIllustration is not null && contentType is not null)
        {
            await _storageService.UploadAuthorIllustration(author, authorIllustration, contentType);
        }
    }

    private async Task ValidateAndThrowOnFailures(long currentAuthorId, RequestUpdateAuthorJson request)
    {
        var result = new UpdateAuthorValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var existsAuthorWithSameName = await _authorReadRepository.ExistActiveAuthorWithName(request.Name.Trim(), currentAuthorId);
            if (existsAuthorWithSameName)
            {
                throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_AUTHOR_NAME_ALREADY_EXISTS]);
            }
        }
    }
}
