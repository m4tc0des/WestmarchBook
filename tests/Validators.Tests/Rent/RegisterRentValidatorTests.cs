using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.Rent.Register;
using WestmarchBook.Exception;

namespace Validators.Tests.Rent;

public class RegisterRentValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestRegisterRentJsonBuilder.Build();

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_When_ExpectedReturnDateIsTomorrow()
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ExpectedReturnDate = DateTime.UtcNow.Date.AddDays(1);

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldBeAnError_When_ReaderIdIsInvalid(long readerId)
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ReaderId = readerId;

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_RENT_READER_REQUIRED));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldBeAnError_When_BookCopyIdIsInvalid(long bookCopyId)
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.BookCopyId = bookCopyId;

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_RENT_BOOK_COPY_REQUIRED));
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_ExpectedReturnDateIsEmpty()
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ExpectedReturnDate = default;

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_RENT_EXPECTED_RETURN_DATE_REQUIRED));
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_ExpectedReturnDateIsToday()
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ExpectedReturnDate = DateTime.UtcNow.Date;

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_RENT_EXPECTED_RETURN_DATE_FUTURE));
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_ExpectedReturnDateIsInThePast()
    {
        var request = RequestRegisterRentJsonBuilder.Build();
        request.ExpectedReturnDate = DateTime.UtcNow.Date.AddDays(-2);

        var validator = new RegisterRentValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_RENT_EXPECTED_RETURN_DATE_FUTURE));
    }
}
