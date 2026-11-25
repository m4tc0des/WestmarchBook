using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.Rent.Return;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.Rent.Return;

public class ReturnBookUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var bookCopy = BookCopyBuilder.Build(copyStatus: CopyStatus.Rented);
        var rent = RentBuilder.Build(bookCopy: bookCopy, rentStatus: RentStatus.Active);
        var rentUpdateBuilder = new IRentUpdateOnlyRepositoryBuilder();
        var bookCopyUpdateBuilder = new IBookCopyUpdateOnlyRepositoryBuilder();
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = CreateUseCase(rent, bookCopy, rentUpdateBuilder, bookCopyUpdateBuilder, unitOfWorkBuilder);

        await useCase.Execute(rent.Id).ShouldNotThrowAsync();

        rent.RentStatus.ShouldBe(RentStatus.Finished);
        rent.ActualReturnDate.ShouldNotBeNull();
        bookCopy.CopyStatus.ShouldBe(CopyStatus.Available);
        rentUpdateBuilder.VerifyUpdateCalled(1);
        bookCopyUpdateBuilder.VerifyUpdateCalled(1);
        unitOfWorkBuilder.VerifyCommitCalled(1);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_RentNotFound()
    {
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();
        var useCase = CreateUseCase(rent: null, bookCopy: null, unitOfWorkBuilder: unitOfWorkBuilder);

        var exception = await useCase.Execute(rentId: 1000).ShouldThrowAsync<NotFoundException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.RENT_NOT_FOUND);
        });
        unitOfWorkBuilder.VerifyCommitCalled(0);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_RentAlreadyReturned()
    {
        var bookCopy = BookCopyBuilder.Build(copyStatus: CopyStatus.Available);
        var rent = RentBuilder.Build(bookCopy: bookCopy, rentStatus: RentStatus.Finished);
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = CreateUseCase(rent, bookCopy, unitOfWorkBuilder: unitOfWorkBuilder);

        var exception = await useCase.Execute(rent.Id).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.RENT_ALREADY_RETURNED);
        });
        unitOfWorkBuilder.VerifyCommitCalled(0);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_BookCopyNotFound()
    {
        var rent = RentBuilder.Build(rentStatus: RentStatus.Active);
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = CreateUseCase(rent, bookCopy: null, unitOfWorkBuilder: unitOfWorkBuilder);

        var exception = await useCase.Execute(rent.Id).ShouldThrowAsync<NotFoundException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_BOOK_COPY_NOT_FOUND);
        });
        unitOfWorkBuilder.VerifyCommitCalled(0);
    }

    private static ReturnBookUseCase CreateUseCase(
        WestmarchBook.Domain.Entities.Rent? rent = null,
        BookCopy? bookCopy = null,
        IRentUpdateOnlyRepositoryBuilder? rentUpdateBuilder = null,
        IBookCopyUpdateOnlyRepositoryBuilder? bookCopyUpdateBuilder = null,
        IUnitOfWorkBuilder? unitOfWorkBuilder = null)
    {
        var rentRepositoryBuilder = rentUpdateBuilder ?? new IRentUpdateOnlyRepositoryBuilder();
        rentRepositoryBuilder.GetById(rent);

        var copyRepositoryBuilder = bookCopyUpdateBuilder ?? new IBookCopyUpdateOnlyRepositoryBuilder();
        copyRepositoryBuilder.GetById(bookCopy);

        var unitOfWork = (unitOfWorkBuilder ?? new IUnitOfWorkBuilder()).BuildInstance();

        return new ReturnBookUseCase(
            rentRepositoryBuilder.Build(),
            copyRepositoryBuilder.Build(),
            unitOfWork);
    }
}
