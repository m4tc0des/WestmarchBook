using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.Register;

public class RegisterBookUseCase : IRegisterBookUseCase
{
    private readonly IBookWriteOnlyRepository _bookRepository;
    private readonly IBookReadOnlyRepository _bookReadOnlyRepository;
    private readonly IPublisherReadOnlyRepository _publisherReadOnlyRepository;
    private readonly IAuthorReadOnlyRepository _authorReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBookUseCase(
        IBookWriteOnlyRepository bookRepository,
        IBookReadOnlyRepository bookReadOnlyRepository,
        IPublisherReadOnlyRepository publisherReadOnlyRepository,
        IAuthorReadOnlyRepository authorReadOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _bookReadOnlyRepository = bookReadOnlyRepository;
        _publisherReadOnlyRepository = publisherReadOnlyRepository;
        _authorReadOnlyRepository = authorReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterBookJson> Execute(RequestRegisterBookJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var book = request.Adapt<Domain.Entities.Book>();

        await _bookRepository.Add(book);

        await _unitOfWork.Commit();

        return new ResponseRegisterBookJson
        {
            Id = book.Id,
            Title = book.Title
        };
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterBookJson request)
    {
        var result = new BookValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var errors = new List<string>();

        var existsBookWithSameTitle = await _bookReadOnlyRepository.ExistActiveBookWithTitle(request.Title);

        if (existsBookWithSameTitle)
        {
            errors.Add(ResourceMessagesException.VALIDATION_BOOK_TITLE_ALREADY_EXISTS);
        }

        var publisherExists = await _publisherReadOnlyRepository.ExistActivePublisherWithId(request.PublisherId);

        if (publisherExists == false)
        {
            errors.Add(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }

        var authorsExist = await _authorReadOnlyRepository.ExistActiveAuthorsWithIds(request.Authors);

        if (authorsExist == false)
        {
            errors.Add(ResourceMessagesException.VALIDATION_AUTHOR_NOT_FOUND);
        }

        if (errors.Count > 0)
        {
            throw new ErrorOnValidationException(errors);
        }
    }
}
