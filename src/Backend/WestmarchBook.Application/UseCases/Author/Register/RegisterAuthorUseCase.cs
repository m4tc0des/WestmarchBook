using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.Register;

public class RegisterAuthorUseCase : IRegisterAuthorUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IAuthorWriteOnlyRepository _authorWriteRepository;
    private readonly IAuthorReadOnlyRepository _authorReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAuthorUseCase(ILoggedUser loggedUser, IAuthorWriteOnlyRepository authorWriteRepository, IAuthorReadOnlyRepository authorReadRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _authorWriteRepository = authorWriteRepository;
        _authorReadRepository = authorReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterAuthorJson> Execute(RequestRegisterAuthorJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var author = request.Adapt<Domain.Entities.Author>();

        await _authorWriteRepository.Add(author);
        await _unitOfWork.Commit();

        return new ResponseRegisterAuthorJson
        {
            Id = author.Id,
            Name = author.Name
        };
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterAuthorJson request)
    {
        var result = new AuthorValidator().Validate(request);
        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var existsAuthor = await _authorReadRepository.ExistActiveAuthorWithName(request.Name);
        if (existsAuthor)
        {
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.VALIDATION_AUTHOR_NAME_ALREADY_EXISTS });
        }
    }
}
