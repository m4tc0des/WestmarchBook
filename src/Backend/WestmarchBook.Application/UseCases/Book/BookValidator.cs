using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Book;

public class BookValidator : AbstractValidator<RequestRegisterBookJson>
{
    public BookValidator()
    {
        RuleFor(book => book.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_TITLE_REQUIRED)
            .MaximumLength(150)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_TITLE_MAX_LENGTH);

        RuleFor(book => book.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_DESCRIPTION_REQUIRED)
            .MaximumLength(250)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_DESCRIPTION_MAX_LENGTH);

        RuleFor(book => book.PublicationDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_PUBLICATION_DATE_INVALID);

        RuleFor(book => book.PublisherId)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_PUBLISHER_REQUIRED);

        RuleFor(book => book.Authors)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHORS_REQUIRED)
            .Must(authors => authors.Distinct().Count() == authors.Count)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHORS_DUPLICATED);

        RuleForEach(book => book.Authors)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_AUTHOR_ID_INVALID);

        RuleFor(book => book.Genres)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRES_REQUIRED)
            .Must(genres => genres.Distinct().Count() == genres.Count)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRES_DUPLICATED);

        RuleForEach(book => book.Genres)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_GENRE_INVALID);

        RuleFor(book => book.Languages)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_LANGUAGES_REQUIRED)
            .Must(languages => languages.Distinct().Count() == languages.Count)
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_LANGUAGES_DUPLICATED);

        RuleForEach(book => book.Languages)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.VALIDATION_BOOK_LANGUAGE_INVALID);
    }
}
