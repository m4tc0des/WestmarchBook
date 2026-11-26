using FluentValidation;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Shared.Validators;

public static class ReaderValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> PhoneRule<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_REQUIRED)
            .Matches(@"^(\+?55\s?)?(\(?\d{2}\)?\s?)?(9\d{4}|\d{4})-?\d{4}$")
            .WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_INVALID);
    }

    public static IRuleBuilderOptions<T, string> CpfRule<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_REQUIRED)
            .Must(BeAValidCpf)
            .WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_INVALID);
    }

    private static bool BeAValidCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        Span<int> digits = stackalloc int[11];
        var count = 0;

        foreach (var c in cpf.AsSpan())
        {
            if (!char.IsAsciiDigit(c))
                continue;

            if (count == 11)
                return false;

            digits[count++] = c - '0';
        }

        if (count != 11)
            return false;

        var firstDigit = digits[0];
        var allEqual = true;
        for (var i = 1; i < 11; i++)
        {
            if (digits[i] != firstDigit)
            {
                allEqual = false;
                break;
            }
        }

        if (allEqual)
            return false;

        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (digits[9] != digit1)
            return false;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return digits[10] == digit2;
    }
}
