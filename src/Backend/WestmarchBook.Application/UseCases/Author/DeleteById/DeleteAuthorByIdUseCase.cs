using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.DeleteById;

public class DeleteAuthorByIdUseCase : IDeleteAuthorByIdUseCase
{
    private readonly IAuthorReadOnlyRepository _authorReadRepository;
    private readonly IAuthorWriteOnlyRepository _authorWriteRepository;
    private readonly IStorageService _storageService;

    public DeleteAuthorByIdUseCase(
        IAuthorReadOnlyRepository authorReadRepository,
        IAuthorWriteOnlyRepository authorWriteRepository,
        IStorageService storageService)
    {
        _authorReadRepository = authorReadRepository;
        _authorWriteRepository = authorWriteRepository;
        _storageService = storageService;
    }

    public async Task Execute(long id)
    {
        var author = await _authorReadRepository.GetById(id);

        if (author is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        if (await _authorReadRepository.HasBooksLinked(id))
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_AUTHOR_HAS_LINKED_BOOKS]);
        }

        var deleted = await _authorWriteRepository.DeleteById(id);

        if (deleted == false)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        await _storageService.DeleteAuthorIllustration(id);
    }
}
