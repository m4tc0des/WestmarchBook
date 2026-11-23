using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Author.Update;

public class UpdateAuthorValidator : AbstractValidator<RequestUpdateAuthorJson>
{
    public UpdateAuthorValidator()
    {
        When(author => author.Name is not null, () =>
        {
            RuleFor(author => author.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_AUTHOR_NAME_REQUIRED)
                .MaximumLength(100)
                .WithMessage(ResourceMessagesException.VALIDATION_AUTHOR_NAME_MAX_LENGTH);
        });
    }
}
