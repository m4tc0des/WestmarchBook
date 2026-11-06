using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.User.Register;

public class RegisterUserAccountValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserAccountValidator()
    {
        RuleFor(user => user.UserName).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_USERNAME_REQUIRED);
        RuleFor(user => user.Password).Password();
        RuleFor(user => user.Email).EmailRule();
    }
}
