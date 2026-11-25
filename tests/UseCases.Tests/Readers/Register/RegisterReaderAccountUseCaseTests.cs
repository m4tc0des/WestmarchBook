using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.Readers.Register;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.Readers.Register;

public class RegisterReaderAccountUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();

        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.FullName);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_FullNameIsEmpty()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.FullName = string.Empty;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_READER_FULLNAME_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_EmailAlreadyExists()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();

        var useCase = CreateUseCase(emailAlreadyExists: request.Email);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_DocumentNumberAlreadyExists()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();

        var useCase = CreateUseCase(documentNumberAlreadyExists: request.DocumentNumber);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_ALREADY_EXISTS);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_CityIsEmpty()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = string.Empty;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_READER_CITY_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_CityIsNotInBahia()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = "São Paulo";

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_READER_CITY_INVALID);
        });
    }

    private static RegisterReaderAccountUseCase CreateUseCase(
        string? emailAlreadyExists = null,
        string? documentNumberAlreadyExists = null)
    {
        var readerWriteOnlyRepository = IReaderWriteOnlyRepositoryBuilder.Build();
        var readerReadOnlyRepositoryBuilder = new IReaderReadOnlyRepositoryBuilder();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        if (!string.IsNullOrEmpty(emailAlreadyExists))
        {
            readerReadOnlyRepositoryBuilder.ExisteActiveReaderWithEmail(emailAlreadyExists);
        }

        if (!string.IsNullOrEmpty(documentNumberAlreadyExists))
        {
            readerReadOnlyRepositoryBuilder.ExisteActiveReaderWithDocumentNumber(documentNumberAlreadyExists);
        }

        return new RegisterReaderAccountUseCase(
            readerWriteOnlyRepository,
            readerReadOnlyRepositoryBuilder.Build(),
            unitOfWork);
    }
}
