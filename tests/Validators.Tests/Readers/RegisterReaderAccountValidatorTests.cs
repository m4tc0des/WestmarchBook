using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.Readers.Register;
using WestmarchBook.Exception;

namespace Validators.Tests.Readers;

public class RegisterReaderAccountValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = "Salvador";

        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Feira de Santana")]
    [InlineData("vitória da conquista")]
    [InlineData("ILHÉUS")]
    [InlineData("Porto Seguro")]
    [InlineData("Juazeiro")]
    public void Success_When_CityIsValidBahiaCity(string validCity)
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = validCity;

        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_ShouldBeAnError_When_CityIsEmpty(string? invalidCity)
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = invalidCity!;

        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_READER_CITY_REQUIRED));
    }

    [Theory]
    [InlineData("São Paulo")]
    [InlineData("Rio de Janeiro")]
    [InlineData("Curitiba")]
    [InlineData("Cidade Inexistente")]
    public void Validate_ShouldBeAnError_When_CityIsNotInBahia(string invalidCity)
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.City = invalidCity;

        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_READER_CITY_INVALID));
    }

    [Theory]
    [InlineData("leitor@gmail.cokm")]
    [InlineData("leitor@hotmail.comm")]
    [InlineData("leitor@dominio.con")]
    [InlineData("leitor@dominio")]
    [InlineData("leitor@.com")]
    [InlineData("leitor@empresa.cokm.br")]
    public void Validate_ShouldBeAnError_When_EmailHasInvalidTldOrFormat(string invalidEmail)
    {
        var request = RequestRegisterReaderJsonBuilder.Build();
        request.Email = invalidEmail;

        var validator = new RegisterReaderAccountValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
    }
}
