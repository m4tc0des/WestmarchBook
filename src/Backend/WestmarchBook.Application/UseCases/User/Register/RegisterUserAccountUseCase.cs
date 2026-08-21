using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Exception.ExceptionBase;

namespace WestmarchBook.Application.UseCases.User.Register;

public class RegisterUserAccountUseCase: IRegisterUserAccountUseCase
{
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserAccountUseCase(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    public void Execute(RequestRegisterUserJson request)
    {
        ValidateAndThrowOnFailures(request);

        var user = request.Adapt<Users>();

        user.Password = _passwordHasher.PasswordHash(request.Password);
    }

    private void ValidateAndThrowOnFailures(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserAccountValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
