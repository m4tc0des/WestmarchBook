using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Author.Register;

public class RegisterAuthorUseCase : IRegisterAuthorUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IAuthorWriteOnlyRepository _authorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAuthorUseCase(ILoggedUser loggedUser, IAuthorWriteOnlyRepository authorRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterAuthorJson> Execute(RequestRegisterAuthorJson request)
    {
        Validate(request);

        var author = request.Adapt<Domain.Entities.Author>();

        await _authorRepository.Add(author);
        await _unitOfWork.Commit();

        return new ResponseRegisterAuthorJson
        {
            Id = author.Id,
            Name = author.Name
        };
    }

    private static void Validate(RequestRegisterAuthorJson request)
    {
        var result = new AuthorValidator().Validate(request);
        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}
