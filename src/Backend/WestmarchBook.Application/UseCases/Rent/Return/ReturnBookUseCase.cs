using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Repositories.Rent;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Rent.Return;

public class ReturnBookUseCase : IReturnBookUseCase
{
    private readonly IRentUpdateOnlyRepository _rentUpdateOnlyRepository;
    private readonly IBookCopyUpdateOnlyRepository _bookCopyUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnBookUseCase(
        IRentUpdateOnlyRepository rentUpdateOnlyRepository,
        IBookCopyUpdateOnlyRepository bookCopyUpdateOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _rentUpdateOnlyRepository = rentUpdateOnlyRepository;
        _bookCopyUpdateOnlyRepository = bookCopyUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long rentId)
    {
        var rent = await _rentUpdateOnlyRepository.GetById(rentId);

        if (rent is null)
        {
            throw new NotFoundException(ResourceMessagesException.RENT_NOT_FOUND);
        }

        if (rent.RentStatus != RentStatus.Active)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.RENT_ALREADY_RETURNED]);
        }

        var bookCopy = await _bookCopyUpdateOnlyRepository.GetById(rent.BookCopyId);

        if (bookCopy is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_FOUND);
        }

        rent.RentStatus = RentStatus.Finished;
        rent.ActualReturnDate = DateTime.UtcNow;
        _rentUpdateOnlyRepository.Update(rent);

        bookCopy.CopyStatus = CopyStatus.Available;
        _bookCopyUpdateOnlyRepository.Update(bookCopy);

        await _unitOfWork.Commit();
    }
}
