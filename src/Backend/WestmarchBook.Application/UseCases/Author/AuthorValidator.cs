using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Author;

public class AuthorValidator: AbstractValidator<RequestRegisterAuthorJson>
{
    public AuthorValidator()
    {
        RuleFor(author => author.Name)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_AUTHOR_NAME_REQUIRED)
            .MaximumLength(100)
            .WithMessage(ResourceMessagesException.VALIDATION_AUTHOR_NAME_MAX_LENGTH);
    }
}
