using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.User.ChangePassword;
using WestmarchBook.Exception;

namespace Validators.Tests.User.ChangePassword;

public class ChangePasswordValidatorTests
{
    [Fact]
    public void Succees()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_When_CurrentPasswordIsEmpty()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();

        request.CurrentPassword = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count().ShouldBe(1);
            error.ShouldContain(x => x.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED));
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Validate_ShouldHaveError_When_NewPasswordIsInvalid(int passwordLenght)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build(passwordLenght);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count().ShouldBe(1);
            error.ShouldContain(x => x.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH));
        });
    }
}
