using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordValidator: AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(request => request.NewPassword).Password();
        RuleFor(request => request.CurrentPassword).Password();
    }
}
