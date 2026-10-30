using FluentValidation;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Readers.Update;

public class UpdateReaderValidator: AbstractValidator<RequestUpdateReaderJson>
{
    public UpdateReaderValidator()
    {
        RuleFor(reader => reader.FullName)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_FULLNAME_REQUIRED);

        RuleFor(reader => reader.Email).EmailRule();

        RuleFor(reader => reader.Phone).PhoneRule();

        RuleFor(reader => reader.DocumentNumber).CpfRule();
    }
}
