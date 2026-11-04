using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Book.Update;

public class UpdateBookValidator : AbstractValidator<RequestUpdateBookJson>
{
    public UpdateBookValidator()
    {
        When(book => book.Title is not null, () =>
        {
            RuleFor(book => book.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_TITLE_REQUIRED)
                .MaximumLength(150)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_TITLE_MAX_LENGTH);
        });

        When(book => book.Description is not null, () =>
        {
            RuleFor(book => book.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_DESCRIPTION_REQUIRED)
                .MaximumLength(250)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_DESCRIPTION_MAX_LENGTH);
        });

        When(book => book.PublicationDate.HasValue, () =>
        {
            RuleFor(book => book.PublicationDate!.Value)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_PUBLICATION_DATE_INVALID);
        });

        When(book => book.PublisherId.HasValue, () =>
        {
            RuleFor(book => book.PublisherId!.Value)
                .GreaterThan(0)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_PUBLISHER_REQUIRED);
        });

        When(book => book.Authors is not null, () =>
        {
            RuleFor(book => book.Authors!)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHORS_REQUIRED)
                .Must(authors => authors.Distinct().Count() == authors.Count)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHORS_DUPLICATED);

            RuleForEach(book => book.Authors!)
                .GreaterThan(0)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHOR_ID_INVALID);
        });

        When(book => book.Genres is not null, () =>
        {
            RuleFor(book => book.Genres!)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRES_REQUIRED)
                .Must(genres => genres.Distinct().Count() == genres.Count)
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRES_DUPLICATED);

            RuleForEach(book => book.Genres!)
                .IsInEnum()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRE_INVALID);
        });

        When(book => book.RegistrationNumbers is not null, () =>
        {
            RuleFor(book => book.RegistrationNumbers!)
                .NotEmpty()
                .WithMessage(ResourceMessagesException.VALIDATION_BOOK_REGISTRATIONNUMBER_REQUIRED);
        });
    }
}
