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

        request.Name = name;

        var userCase = new RegisterUserAccountValidator();
        var result = userCase.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
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
}
