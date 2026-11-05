using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Repositories.Reader;
using WestmarchBook.Domain.Repositories.Rent;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Rent.Register;

public class RegisterRentUseCase : IRegisterRentUseCase
{
    private readonly IRentWriteOnlyRepository _rentWriteOnlyRepository;
    private readonly IBookCopyUpdateOnlyRepository _bookCopyUpdateOnlyRepository;
    private readonly IReaderReadOnlyRepository _readerReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterRentUseCase(
        IRentWriteOnlyRepository rentWriteOnlyRepository,
        IBookCopyUpdateOnlyRepository bookCopyUpdateRepository,
        IReaderReadOnlyRepository readerReadOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _rentWriteOnlyRepository = rentWriteOnlyRepository;
        _bookCopyUpdateOnlyRepository = bookCopyUpdateRepository;
        _readerReadOnlyRepository = readerReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterRentJson> Execute(RequestRegisterRentJson request)
    {
        ValidateAndThrowOnFailures(request);

        var reader = await _readerReadOnlyRepository.GetById(request.ReaderId);
        if (reader is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_READER_NOT_FOUND);
        }

        var bookCopy = await _bookCopyUpdateOnlyRepository.GetById(request.BookCopyId);
        if (bookCopy is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_FOUND);
        }

        if (bookCopy.CopyStatus != CopyStatus.Available)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_AVAILABLE]);
        }

        var rent = new Domain.Entities.Rent
        {
            ReaderId = request.ReaderId,
            BookCopyId = request.BookCopyId,
            ExpectedReturnDate = request.ExpectedReturnDate,
            RentDate = DateTime.UtcNow,
            RentStatus = RentStatus.Active
        };

        bookCopy.CopyStatus = CopyStatus.Rented;
        _bookCopyUpdateOnlyRepository.Update(bookCopy);

        await _rentWriteOnlyRepository.AddRent(rent);

        await _unitOfWork.Commit();

        return new ResponseRegisterRentJson
        {
            RentId = rent.Id
        };
    }

    private static void ValidateAndThrowOnFailures(RequestRegisterRentJson request)
    {
        var validator = new RegisterRentValidator();
        var results = validator.Validate(request);

        if (results.IsValid == false)
        {
            var errorMessages = results.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
