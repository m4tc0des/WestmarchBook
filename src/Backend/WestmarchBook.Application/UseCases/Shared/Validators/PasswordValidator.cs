using FluentValidation;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Shared.Validators;

public static class PasswordValidator
{
    extension<TRequest>(IRuleBuilderInitial<TRequest, string> ruleBuilder)
    {
        internal IRuleBuilderOptions<TRequest, string> Password()
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED)
                .MinimumLength(8)
                .WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH)
                .Must(password => password.Any(char.IsLower))
                .WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_LOWERCASE)
                .Must(password => password.Any(char.IsUpper))
                .WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_UPPERCASE)
                .Must(password => password.Any(x => !char.IsLetterOrDigit(x)))
                .WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_SPECIAL_CHARACTER);
        }
    }
}
