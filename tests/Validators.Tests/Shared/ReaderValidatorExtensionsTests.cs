using CommonTestUtilities.ErrorsClassData;
using FluentValidation;
using Shouldly;
using WestmarchBook.Application.UseCases.Shared.Validators;
using WestmarchBook.Exception;

namespace Validators.Tests.Shared;

public class ReaderValidatorExtensionsTests
{
    private sealed class TestModel
    {
        public string Phone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }

    private sealed class ReaderExtensionsValidator : AbstractValidator<TestModel>
    {
        public ReaderExtensionsValidator()
        {
            RuleFor(x => x.Phone).PhoneRule();
            RuleFor(x => x.Cpf).CpfRule();
        }
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void PhoneRule_ShouldStopCascade_When_PhoneIsEmpty(string phone)
    {
        var validator = new ReaderExtensionsValidator();
        var model = new TestModel
        {
            Phone = phone,
            Cpf = "52998224725"
        };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_READER_PHONE_REQUIRED);
    }

    [Theory]
    [ClassData(typeof(EmptyNullOrBlankSpace))]
    public void CpfRule_ShouldStopCascade_When_CpfIsEmpty(string cpf)
    {
        var validator = new ReaderExtensionsValidator();
        var model = new TestModel
        {
            Phone = "(71) 99999-8888",
            Cpf = cpf
        };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_REQUIRED);
    }

    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void CpfRule_ShouldSucceed_When_CpfIsValid(string validCpf)
    {
        var validator = new ReaderExtensionsValidator();
        var model = new TestModel
        {
            Phone = "(71) 99999-8888",
            Cpf = validCpf
        };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("52998224720")]
    [InlineData("529982247251")]
    [InlineData("5299822472")]
    public void CpfRule_ShouldFail_When_CpfIsInvalid(string invalidCpf)
    {
        var validator = new ReaderExtensionsValidator();
        var model = new TestModel
        {
            Phone = "(71) 99999-8888",
            Cpf = invalidCpf
        };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_INVALID);
    }
}
