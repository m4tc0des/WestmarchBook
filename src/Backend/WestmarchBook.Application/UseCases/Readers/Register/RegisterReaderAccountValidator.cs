using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Readers.Register;

public class RegisterReaderAccountValidator : AbstractValidator<RequestRegisterReaderJson>
{
    public RegisterReaderAccountValidator()
    {
        RuleFor(reader => reader.FullName)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_FULLNAME_REQUIRED);

        RuleFor(reader => reader.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED)
            .EmailAddress().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_INVALID);

        RuleFor(reader => reader.Phone)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_REQUIRED)
            .Matches(@"^(\+?55\s?)?(\(?\d{2}\)?\s?)?(9\d{4}|\d{4})-?\d{4}$")
            .WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_INVALID);

        RuleFor(reader => reader.DocumentNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_REQUIRED)
            .Must(BeAValidCpf)
            .WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_INVALID);
    }

    private bool BeAValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }
}