using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.GetById;

public class GetAuthorByIdUseCase : IGetAuthorByIdUseCase
{
    private readonly IAuthorReadOnlyRepository _authorReadRepository;

    public GetAuthorByIdUseCase(IAuthorReadOnlyRepository authorReadRepository)
    {
        _authorReadRepository = authorReadRepository;
    }

    public async Task<ResponseAuthorJson> Execute(long id)
    {
        var author = await _authorReadRepository.GetById(id);

        if (author == null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        return author.Adapt<ResponseAuthorJson>();
    }
}
