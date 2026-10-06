using CommonTestUtilities.ErrorsClassData;
using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Exception;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldBeAnError_When_NameIsEmpty(string name)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.UserName = name;

        var userCase = new RegisterUserAccountValidator();
        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_USERNAME_REQUIRED));
        });
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldBeAnError_When_EmailIsEmpty(string email)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = email;

        var userCase = new RegisterUserAccountValidator();
        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_EmailIsInvalid()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = "email.com";

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
        });
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldBeAnError_When_PasswordIsEmpty(string password)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Password = password;

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED));
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Validate_ShouldBeAnError_When_PasswordHasInsufficientCharacters(int passwordLength)
    {
        var request = RequestRegisterUserJsonBuilder.Build(passwordLength);

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_PasswordDoesNotHaveLowercaseLetter()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Password = "@PASSWORD123";

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_LOWERCASE));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_PasswordDoesNotHaveUppercaseLetter()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Password = "@password123";

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_UPPERCASE));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_PasswordDoesNotHaveSpecialCharacter()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Password = "Password123";

        var userCase = new RegisterUserAccountValidator();

        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_SPECIAL_CHARACTER));
        });
    }
}
