using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.Rent.Register;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.Rent.Register;

public class RegisterRentUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterRentJsonBuilder.Build();

        var reader = ReaderBuilder.Build();
        reader.Id = request.ReaderId;

        var bookCopy = BookCopyBuilder.Build(copyStatus: CopyStatus.Available);
        bookCopy.Id = request.BookCopyId;

        var useCase = CreateUseCase(reader: reader, bookCopy: bookCopy);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        bookCopy.CopyStatus.ShouldBe(CopyStatus.Rented);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_ReaderNotFound()
    {
        var request = RequestRegisterRentJsonBuilder.Build();

        var bookCopy = BookCopyBuilder.Build(copyStatus: CopyStatus.Available);
        bookCopy.Id = request.BookCopyId;

        var useCase = CreateUseCase(reader: null, bookCopy: bookCopy);

        var exception = await useCase.Execute(request).ShouldThrowAsync<NotFoundException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_READER_NOT_FOUND);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_BookCopyNotFound()
    {
        var request = RequestRegisterRentJsonBuilder.Build();

        var reader = ReaderBuilder.Build();
        reader.Id = request.ReaderId;

        var useCase = CreateUseCase(reader: reader, bookCopy: null);

        var exception = await useCase.Execute(request).ShouldThrowAsync<NotFoundException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_FOUND);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_BookCopyNotAvailable()
    {
        var request = RequestRegisterRentJsonBuilder.Build();

        var reader = ReaderBuilder.Build();
        reader.Id = request.ReaderId;

        var bookCopy = BookCopyBuilder.Build(copyStatus: CopyStatus.Rented);
        bookCopy.Id = request.BookCopyId;

        var useCase = CreateUseCase(reader: reader, bookCopy: bookCopy);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_AVAILABLE);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_RequestIsInvalid()
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ReaderId = 0;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_RENT_READER_REQUIRED);
        });
    }

    private static RegisterRentUseCase CreateUseCase(Reader? reader = null, BookCopy? bookCopy = null)
    {
        var rentWriteRepository = new IRentWriteOnlyRepositoryBuilder().Build();

        var readerReadOnlyRepositoryBuilder = new IReaderReadOnlyRepositoryBuilder();
        if (reader is not null)
        {
            readerReadOnlyRepositoryBuilder.GetById(reader);
        }

        var bookCopyUpdateRepositoryBuilder = new IBookCopyUpdateOnlyRepositoryBuilder();
        if (bookCopy is not null)
        {
            bookCopyUpdateRepositoryBuilder.GetById(bookCopy);
        }

        var unitOfWork = IUnitOfWorkBuilder.Build();

        return new RegisterRentUseCase(
            rentWriteRepository,
            bookCopyUpdateRepositoryBuilder.Build(),
            readerReadOnlyRepositoryBuilder.Build(),
            unitOfWork);
    }
}
