using System.Net.Mail;
using System.Text.RegularExpressions;
using FluentValidation;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Shared.Validators;

public static partial class EmailValidator
{
    [GeneratedRegex(
        @"^(?!\.)(?!.*\.\.)[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(?<!\.)@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,24}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex StrictEmailRegex();

    public static IRuleBuilderOptions<T, string> EmailRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        if (ruleBuilder is IRuleBuilderInitial<T, string> initialRuleBuilder)
            initialRuleBuilder.Cascade(CascadeMode.Stop);

        return ruleBuilder
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED)
            .Must(BeAValidEmail).WithMessage(ResourceMessagesException.VALIDATION_EMAIL_INVALID);
    }

    private static bool BeAValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmed = email.Trim();
        if (!StrictEmailRegex().IsMatch(trimmed))
            return false;

        return MailAddress.TryCreate(trimmed, out var mailAddress)
            && string.Equals(mailAddress.Address, trimmed, StringComparison.OrdinalIgnoreCase);
    }
}
