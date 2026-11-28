using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Genre.Update;

public class UpdateGenreValidator : AbstractValidator<RequestUpdateGenreJson>
{
    public UpdateGenreValidator()
    {
        RuleFor(genre => genre.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_GENRE_NAME_REQUIRED)
            .MaximumLength(100)
            .WithMessage(ResourceMessagesException.VALIDATION_GENRE_NAME_MAX_LENGTH);
    }
}
