using WestmarchBook.Application.UseCases.Book.Register;
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

        book.Title = request.Title.Trim();
        book.Description = request.Description.Trim();
        book.PublicationDate = request.PublicationDate;
        book.PublisherId = request.PublisherId;
        book.Genres = request.Genres.Distinct().Select(g => (Domain.Enums.Genre)g).ToList();
        book.Languages = request.Languages.Distinct().Select(l => (Domain.Enums.Language)l).ToList();

        book.BookAuthors.Clear();
        foreach (var authorId in request.Authors.Distinct())
        {
            book.BookAuthors.Add(new BookAuthor
            {
                BookId = book.Id,
                AuthorId = authorId
            });
        }

        _bookUpdateRepository.Update(book);

        await _unitOfWork.Commit();
    }

    private async Task ValidateAndThrowOnFailures(long currentBookId, RequestUpdateBookJson request)
    {
        var result = new BookValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var errors = new List<string>();

        var existsBookWithSameTitle = await _bookReadOnlyRepository.ExistActiveBookWithTitle(request.Title, currentBookId);
        if (existsBookWithSameTitle)
        {
            errors.Add(ResourceMessagesException.VALIDATION_BOOK_TITLE_ALREADY_EXISTS);
        }

        var publisherExists = await _publisherReadOnlyRepository.ExistActivePublisherWithId(request.PublisherId);
        if (!publisherExists)
        {
            errors.Add(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }

        var authorsExist = await _authorReadOnlyRepository.ExistActiveAuthorsWithIds(request.Authors);
        if (!authorsExist)
        {
            errors.Add(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        if (errors.Count > 0)
        {
            throw new ErrorOnValidationException(errors);
        }
    }
}
