using CommonTestUtilities.ErrorsClassData;
using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.Publisher.Update;
using WestmarchBook.Exception;

namespace Validators.Tests.Publisher;

public class UpdatePublisherValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestUpdatePublisherJsonBuilder.Build();

        var validator = new UpdatePublisherValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void Validate_ShouldBeAnError_When_NameIsEmpty(string name)
    {
        var request = RequestUpdatePublisherJsonBuilder.Build();
        request.Name = name;

        var validator = new UpdatePublisherValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldBeAnError_When_NameExceedsMaxLength()
    {
        var request = RequestUpdatePublisherJsonBuilder.Build();
        request.Name = new string('a', 101);

        var validator = new UpdatePublisherValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_MAX_LENGTH));
        });
    }
}
