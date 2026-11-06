using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.User.Update;

public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator()
    {
        RuleFor(request => request.UserName).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_USERNAME_REQUIRED);
        RuleFor(request => request.Email).EmailRule();
    }
}
