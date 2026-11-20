using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.DeleteIllustration;

public class DeleteIllustrationUseCase : IDeleteIllustrationUseCase
{
    private readonly IAuthorUpdateOnlyRepository _authorUpdateRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIllustrationUseCase(
        IAuthorUpdateOnlyRepository authorUpdateRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _authorUpdateRepository = authorUpdateRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long authorId)
    {
        var author = await _authorUpdateRepository.GetById(authorId);

        if (author is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        if (author.HasImage)
        {
            await _storageService.DeleteAuthorIllustration(authorId);

            author.HasImage = false;
            _authorUpdateRepository.Update(author);

            await _unitOfWork.Commit();
        }
    }
}
