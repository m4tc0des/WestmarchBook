using FluentValidation.Results;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Reader;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Readers.Update;

public class UpdateReaderUseCase : IUpdateReaderUseCase
{
    private readonly IReaderReadOnlyRepository _readerReadOnlyRepository;
    private readonly IReaderUpdateOnlyRepository _readerUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReaderUseCase(
        IReaderReadOnlyRepository readerReadOnlyRepository,
        IReaderUpdateOnlyRepository readerUpdateOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _readerReadOnlyRepository = readerReadOnlyRepository;
        _readerUpdateOnlyRepository = readerUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long id, RequestUpdateReaderJson request)
    {
        var reader = await _readerReadOnlyRepository.GetById(id);

        if (reader is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_READER_NOT_FOUND);
        }

        await ValidateAndThrowOnFailures(request, id);

        reader.FullName = request.FullName.Trim();
        reader.Email = request.Email.Trim();
        reader.Phone = request.Phone.Trim();
        reader.DocumentNumber = SanitizeDigits(request.DocumentNumber);

        _readerUpdateOnlyRepository.UpdateReader(reader);

        await _unitOfWork.Commit();
    }

    private async Task ValidateAndThrowOnFailures(RequestUpdateReaderJson request, long currentReaderId)
    {
        var validator = new UpdateReaderValidator();

        var result = validator.Validate(request);

        var sanitizedEmail = (request.Email ?? string.Empty).Trim();
        var emailExists = await _readerReadOnlyRepository.ExisteActiveReaderWithEmail(sanitizedEmail, currentReaderId);

        if (emailExists)
        {
            result.Errors.Add(new ValidationFailure("email", ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS));
        }

        var sanitizedDocument = SanitizeDigits(request.DocumentNumber);
        if (!string.IsNullOrWhiteSpace(sanitizedDocument))
        {
            var documentExists = await _readerReadOnlyRepository.ExisteActiveReaderWithDocumentNumber(sanitizedDocument, currentReaderId);
            if (documentExists)
            {
                result.Errors.Add(new ValidationFailure("documentNumber", ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_ALREADY_EXISTS));
            }
        }

        if (result.IsValid == false)
        {
            var errorMessage = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessage);
        }
    }

    private static string SanitizeDigits(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : new string(value.Where(char.IsDigit).ToArray());
    }
}
