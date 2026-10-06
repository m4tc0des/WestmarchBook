using CommonTestUtilities.ErrorsClassData;
using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.User.Update;
using WestmarchBook.Exception;

namespace Validators.Tests.User.Update;

public class UpdateUserValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldHaveError_When_UserNameIsEmpty(string userName)
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        request.UserName = userName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count().ShouldBe(1);
            error.ShouldContain(x => x.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_USERNAME_REQUIRED));
        });
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldHaveError_When_EmailIsEmpty(string email)
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        request.Email = email;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count().ShouldBe(1);
            error.ShouldContain(x => x.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_When_EmailIsInvalid()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        request.Email = "api.com";

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count().ShouldBe(1);
            error.ShouldContain(x => x.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
        });
    }
}
