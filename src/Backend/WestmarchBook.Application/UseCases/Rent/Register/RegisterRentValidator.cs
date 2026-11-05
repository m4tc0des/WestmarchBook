using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Rent.Register;

public class RegisterRentValidator : AbstractValidator<RequestRegisterRentJson>
{
    public RegisterRentValidator()
    {
        RuleFor(rent => rent.ReaderId)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.VALIDATION_RENT_READER_REQUIRED);

        RuleFor(rent => rent.BookCopyId)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.VALIDATION_RENT_BOOK_COPY_REQUIRED);

        RuleFor(rent => rent.ExpectedReturnDate)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_RENT_EXPECTED_RETURN_DATE_REQUIRED)
            .Must(expectedReturnDate => expectedReturnDate.Date >= DateTime.UtcNow.Date.AddDays(1))
            .WithMessage(ResourceMessagesException.VALIDATION_RENT_EXPECTED_RETURN_DATE_FUTURE);
    }
}
