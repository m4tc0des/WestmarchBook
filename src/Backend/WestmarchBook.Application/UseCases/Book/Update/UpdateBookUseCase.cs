using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.Update;

public class UpdateBookUseCase : IUpdateBookUseCase
{
    private readonly IBookUpdateOnlyRepository _bookUpdateRepository;
    private readonly IBookReadOnlyRepository _bookReadOnlyRepository;
    private readonly IPublisherReadOnlyRepository _publisherReadOnlyRepository;
    private readonly IAuthorReadOnlyRepository _authorReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookUseCase(
        IBookUpdateOnlyRepository bookUpdateRepository,
        IBookReadOnlyRepository bookReadOnlyRepository,
        IPublisherReadOnlyRepository publisherReadOnlyRepository,
        IAuthorReadOnlyRepository authorReadOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _bookUpdateRepository = bookUpdateRepository;
        _bookReadOnlyRepository = bookReadOnlyRepository;
        _publisherReadOnlyRepository = publisherReadOnlyRepository;
        _authorReadOnlyRepository = authorReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long id, RequestUpdateBookJson request)
    {
        var book = await _bookUpdateRepository.GetById(id);

        if (book is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_NOT_FOUND);
        }

        await ValidateAndThrowOnFailures(id, request);

        if (request.Title is not null)
        {
            book.Title = request.Title.Trim();
        }

        if (request.Description is not null)
        {
            book.Description = request.Description.Trim();
        }

        if (request.PublicationDate.HasValue)
        {
            book.PublicationDate = request.PublicationDate.Value;
        }

        if (request.PublisherId.HasValue)
        {
            book.PublisherId = request.PublisherId.Value;
        }

        if (request.Genres is not null)
        {
            book.Genres = request.Genres.Distinct().Select(g => (Domain.Enums.Genre)g).ToList();
        }


        if (request.Authors is not null)
        {
            book.BookAuthors.Clear();
            foreach (var authorId in request.Authors.Distinct())
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                });
            }
        }

        if (request.RegistrationNumbers is not null)
        {
            var newRegNumbers = request.RegistrationNumbers
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var copiesToRemove = book.BookCopies
                .Where(c => !newRegNumbers.Any(nr => string.Equals(nr, c.RegistrationNumber, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var copy in copiesToRemove)
            {
                book.BookCopies.Remove(copy);
            }

            foreach (var regNum in newRegNumbers)
            {
                if (!book.BookCopies.Any(c => string.Equals(c.RegistrationNumber, regNum, StringComparison.OrdinalIgnoreCase)))
                {
                    book.BookCopies.Add(new BookCopy
                    {
                        BookId = book.Id,
                        RegistrationNumber = regNum,
                        CopyStatus = Domain.Enums.CopyStatus.Available
                    });
                }
            }
        }

        _bookUpdateRepository.Update(book);

        await _unitOfWork.Commit();
    }

    private async Task ValidateAndThrowOnFailures(long currentBookId, RequestUpdateBookJson request)
    {
        var result = new UpdateBookValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var existsBookWithSameTitle = await _bookReadOnlyRepository.ExistActiveBookWithTitle(request.Title.Trim(), currentBookId);
            if (existsBookWithSameTitle)
            {
                errors.Add(ResourceMessagesException.VALIDATION_BOOK_TITLE_ALREADY_EXISTS);
            }
        }

        if (request.PublisherId.HasValue)
        {
            var publisherExists = await _publisherReadOnlyRepository.ExistActivePublisherWithId(request.PublisherId.Value);
            if (!publisherExists)
            {
                errors.Add(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
            }
        }

        if (request.Authors is not null && request.Authors.Count > 0)
        {
            var authorsExist = await _authorReadOnlyRepository.ExistActiveAuthorsWithIds(request.Authors);
            if (!authorsExist)
            {
                errors.Add(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
            }
        }

        if (errors.Count > 0)
        {
            throw new ErrorOnValidationException(errors);
        }
    }
}
