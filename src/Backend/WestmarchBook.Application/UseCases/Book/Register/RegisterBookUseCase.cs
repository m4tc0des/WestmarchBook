using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Book.Register;

public class RegisterBookUseCase : IRegisterBookUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IBookWriteOnlyRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBookUseCase(ILoggedUser loggedUser, IBookWriteOnlyRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterBookJson> Execute(RequestRegisterBookJson request)
    {
        Validate(request);

        var book = request.Adapt<Domain.Entities.Book>();

        book.UserId = _loggedUser.GetUserId();

        await _bookRepository.Add(book);

        await _unitOfWork.Commit();

        return new ResponseRegisterBookJson
        {
            Id = book.Id,
            Title = book.Title
        };
    }

    private static void Validate(RequestRegisterBookJson request)
    {
        var result = new BookValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}
