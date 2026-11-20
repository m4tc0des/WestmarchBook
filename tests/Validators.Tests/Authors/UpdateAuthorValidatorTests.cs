using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.Author.Update;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace Validators.Tests.Authors;

public class UpdateAuthorValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestUpdateAuthorJsonBuilder.Build();

        var validator = new UpdateAuthorValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_When_NameIsNull()
    {
        var request = new RequestUpdateAuthorJson { Name = null };

        var validator = new UpdateAuthorValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldBeAnError_When_NameIsEmptyOrWhitespace(string name)
    {
        var request = new RequestUpdateAuthorJson { Name = name };

        var validator = new UpdateAuthorValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_AUTHOR_NAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_NameExceedsMaxLength()
    {
        var request = new RequestUpdateAuthorJson { Name = new string('a', 101) };

        var validator = new UpdateAuthorValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_AUTHOR_NAME_MAX_LENGTH));
        });
    }
}
