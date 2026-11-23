using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.GetById;

public class GetAuthorByIdUseCase : IGetAuthorByIdUseCase
{
    private readonly IAuthorReadOnlyRepository _authorReadRepository;
    private readonly IStorageService _storageService;

    public GetAuthorByIdUseCase(IAuthorReadOnlyRepository authorReadRepository, IStorageService storageService)
    {
        _authorReadRepository = authorReadRepository;
        _storageService = storageService;
    }

    public async Task<ResponseAuthorJson> Execute(long id)
    {
        var author = await _authorReadRepository.GetById(id);

        if (author is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        var response = author.Adapt<ResponseAuthorJson>();
        response.ImageUrl = author.HasImage ? _storageService.GetAuthorIllustrationUrl(author.Id) : string.Empty;

        return response;
    }
}
