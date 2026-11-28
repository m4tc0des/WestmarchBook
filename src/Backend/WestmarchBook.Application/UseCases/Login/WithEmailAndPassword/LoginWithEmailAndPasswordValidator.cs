using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordValidator: AbstractValidator<RequestLoginJson>
{
    public LoginWithEmailAndPasswordValidator()
    {
        RuleFor(user => user.Email).EmailRule();
        RuleFor(user => user.Password).Password();
    }
}
