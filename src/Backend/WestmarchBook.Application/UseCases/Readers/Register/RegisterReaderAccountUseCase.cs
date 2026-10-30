using FluentValidation.Results;
using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Reader;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Readers.Register;

public class RegisterReaderAccountUseCase : IRegisterReaderAccountUseCase
{
    private readonly IReaderWriteOnlyRepository _readerWriteRepository;
    private readonly IReaderReadOnlyRepository _readerReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterReaderAccountUseCase(
        IReaderWriteOnlyRepository readerWriteRepository,
        IReaderReadOnlyRepository readerReadOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _readerWriteRepository = readerWriteRepository;
        _readerReadOnlyRepository = readerReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterReaderJson> Execute(RequestRegisterReaderJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var reader = request.Adapt<Domain.Entities.Reader>();
        reader.FullName = request.FullName.Trim();
        reader.Email = request.Email.Trim();
        reader.Phone = request.Phone.Trim();
        reader.DocumentNumber = SanitizeDigits(request.DocumentNumber);

        await _readerWriteRepository.Add(reader);

        await _unitOfWork.Commit();

        return new ResponseRegisterReaderJson
        {
            Name = reader.FullName
        };
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterReaderJson request)
    {
        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        var sanitizedEmail = (request.Email ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(sanitizedEmail))
        {
            var emailExists = await _readerReadOnlyRepository.ExisteActiveReaderWithEmail(sanitizedEmail, 0);
            if (emailExists)
            {
                result.Errors.Add(new ValidationFailure("email", ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS));
            }
        }

        var sanitizedDocument = SanitizeDigits(request.DocumentNumber);
        if (!string.IsNullOrWhiteSpace(sanitizedDocument))
        {
            var documentExists = await _readerReadOnlyRepository.ExisteActiveReaderWithDocumentNumber(sanitizedDocument, 0);
            if (documentExists)
            {
                result.Errors.Add(new ValidationFailure("documentNumber", ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_ALREADY_EXISTS));
            }
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private static string SanitizeDigits(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : new string(value.Where(char.IsDigit).ToArray());
    }
}
