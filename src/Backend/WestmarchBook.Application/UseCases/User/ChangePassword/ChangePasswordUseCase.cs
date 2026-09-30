using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserUpdateOnlyRepository _updateOnlyRepository;

    public ChangePasswordUseCase(ILoggedUser loggedUser, IPasswordHasher passwordHasher, IUserUpdateOnlyRepository updateOnlyRepository)
    {
        _loggedUser = loggedUser;
        _passwordHasher = passwordHasher;
        _updateOnlyRepository = updateOnlyRepository;
    }

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.GetProfile();

        Validate(request, loggedUser);

        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);

        await _updateOnlyRepository.UpdatePassword(loggedUser.Id, hashedPassword);
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if (_passwordHasher.VerifyPassword(request.CurrentPassword, loggedUser.Password) == false)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.VALIDATION_CURRENT_PASSWORD));    
        }

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
        }
    }
}
