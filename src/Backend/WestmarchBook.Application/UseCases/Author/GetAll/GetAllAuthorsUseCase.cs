using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Author;

namespace WestmarchBook.Application.UseCases.Author.GetAll;

public class GetAllAuthorsUseCase : IGetAllAuthorsUseCase
{
    private readonly IAuthorReadOnlyRepository _authorReadRepository;

    public GetAllAuthorsUseCase(IAuthorReadOnlyRepository authorReadRepository)
    {
        _authorReadRepository = authorReadRepository;
    }

    public async Task<IList<ResponseAuthorJson>> Execute()
    {
        var authors = await _authorReadRepository.GetAllActive();

        return authors.Adapt<IList<ResponseAuthorJson>>();
    }
}
