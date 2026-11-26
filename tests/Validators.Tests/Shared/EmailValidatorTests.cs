using CommonTestUtilities.ErrorsClassData;
using FluentValidation;
using Shouldly;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Exception;

namespace Validators.Tests.Shared;

public class EmailValidatorTests
{
    private sealed class TestModel
    {
        public string Email { get; set; } = string.Empty;
    }

    private sealed class ChainedEmailValidator : AbstractValidator<TestModel>
    {
        public ChainedEmailValidator()
        {
            RuleFor(x => x.Email)
                .Must(email => !email.Contains(' '))
                .WithMessage("no-spaces")
                .EmailRule();
        }
    }

    private sealed class BasicEmailValidator : AbstractValidator<TestModel>
    {
        public BasicEmailValidator()
        {
            RuleFor(x => x.Email).EmailRule();
        }
    }

    [Fact]
    public void EmailRule_ShouldChainAfterExistingRuleBuilderOptions()
    {
        var validator = new ChainedEmailValidator();

        var result = validator.Validate(new TestModel { Email = "leitor@empresa.coop.br" });

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void EmailRule_ShouldStopCascade_When_EmailIsEmpty(string email)
    {
        var validator = new BasicEmailValidator();

        var result = validator.Validate(new TestModel { Email = email });

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED);
    }

    [Theory]
    [InlineData("usuario@empresa.museum")]
    [InlineData("usuario@startup.tech")]
    [InlineData("usuario@empresa.eng.br")]
    [InlineData("usuario@cooperativa.coop.br")]
    public void EmailRule_ShouldSucceed_When_EmailHasValidFormat(string validEmail)
    {
        var validator = new BasicEmailValidator();

        var result = validator.Validate(new TestModel { Email = validEmail });

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("usuario@dominio.")]
    [InlineData("usuario@-dominio.com")]
    [InlineData("usuario..teste@dominio.com")]
    [InlineData("usuario@dominio")]
    [InlineData("usuario@.com")]
    [InlineData("usuario@dominio.c")]
    public void EmailRule_ShouldFail_When_EmailHasInvalidFormat(string invalidEmail)
    {
        var validator = new BasicEmailValidator();

        var result = validator.Validate(new TestModel { Email = invalidEmail });

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_EMAIL_INVALID);
    }
}
