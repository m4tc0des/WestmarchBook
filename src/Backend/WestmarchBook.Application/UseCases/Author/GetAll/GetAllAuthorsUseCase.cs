using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Services.StorageService;

namespace WestmarchBook.Application.UseCases.Author.GetAll;

public class GetAllAuthorsUseCase : IGetAllAuthorsUseCase
{
    private readonly IAuthorReadOnlyRepository _authorReadRepository;
    private readonly IStorageService _storageService;

    public GetAllAuthorsUseCase(IAuthorReadOnlyRepository authorReadRepository, IStorageService storageService)
    {
        _authorReadRepository = authorReadRepository;
        _storageService = storageService;
    }

    public async Task<IList<ResponseAuthorJson>> Execute()
    {
        var authors = await _authorReadRepository.GetAllActive();

        var response = new List<ResponseAuthorJson>(authors.Count);
        foreach (var author in authors)
        {
            var item = author.Adapt<ResponseAuthorJson>();
            item.ImageUrl = author.HasImage ? _storageService.GetAuthorIllustrationUrl(author.Id) : string.Empty;
            response.Add(item);
        }

        return response;
    }
}
