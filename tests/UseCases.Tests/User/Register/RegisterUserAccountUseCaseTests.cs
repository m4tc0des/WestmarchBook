using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Shouldly;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionBase;

namespace UseCases.Tests.User.Register;

public class RegisterUserAccountUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();
        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Tokens.AccessToken.ShouldBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "<Is Null cause is a test>")]
    public async Task Validate_ShouldHaveAnError_When_NameIsEmpty(string name)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Name = name;

        var useCase = CreateUseCase();
        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessage().ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "<Is Null cause is a test>")]
    public async Task Validate_ShouldHaveAnError_When_EmailIsEmpty(string email)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = email;

        var useCase = CreateUseCase();
        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessage().ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "<Is Null cause is a test>")]
    public async Task Validate_ShouldHaveAnError_When_PasswordIsEmpty(string password)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Password = password;

        var useCase = CreateUseCase();
        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessage().ShouldSatisfyAllConditions(error =>
        {
            error.Count.ShouldBe(1);
            error.ShouldContain(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
        });
    }

    private RegisterUserAccountUseCase CreateUseCase()
    {
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userWriteOnlyRepository = IUserWriteOnlyRepositoryBuilder.Build();
        var userReadOnlyRepository = new IUserReadOnlyRepositoryBuilder().Build();
        var passwordHasher = new IPasswordHasherBuilder().Build();

        return new RegisterUserAccountUseCase(passwordHasher, userWriteOnlyRepository, userReadOnlyRepository, unitOfWork);
    }
}
