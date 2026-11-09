using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class ResetPasswordValidator: AbstractValidator<RequestResetPasswordJson>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.NewPassword).Password();
    }
}
