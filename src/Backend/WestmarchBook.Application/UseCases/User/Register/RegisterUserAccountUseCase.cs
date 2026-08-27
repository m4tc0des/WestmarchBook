using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.User.Register;

public class RegisterUserAccountUseCase : IRegisterUserAccountUseCase
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserWriteOnlyRepository _userWriteRepository;
    private readonly IUserReadOnlyRepository _userReadRepository;

    public RegisterUserAccountUseCase(IPasswordHasher passwordHasher,
        IUserWriteOnlyRepository userWriteRepository,
        IUserReadOnlyRepository userReadRepository,
        IUnitOfWork unitOfWork)
    {
        _userWriteRepository = userWriteRepository;
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseRegisterUserJson> Execute(RequestRegisterUserJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var user = request.Adapt<Domain.Entities.User>();

        user.Password = _passwordHasher.PasswordHash(request.Password);

        await _userWriteRepository.Add(user);

        await _unitOfWork.Commit();

        return new ResponseRegisterUserJson
        {
            Name = user.Name
        };
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserAccountValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }

        var userExists = await _userReadRepository.ExisteActiveUserWithEmail(request.Email);

        if (userExists)
        {
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS });
        } 
    }
}
