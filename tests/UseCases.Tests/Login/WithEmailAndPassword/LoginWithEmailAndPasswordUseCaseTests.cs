using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();

        request.Email = user.Email;

        var useCase = CreateUseCase(request.Password, user);
        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Tokens.AccessToken.ShouldBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_UserDontExist()
    {
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase();
        var exception = await useCase.Execute(request).ShouldThrowAsync<InvalidLoginException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Unauthorized);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_LOGIN_INVALID);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_PasswordIsInvalid()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();

        request.Email = user.Email;

        var useCase = CreateUseCase(user:user);
        var exception = await useCase.Execute(request).ShouldThrowAsync<InvalidLoginException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.Unauthorized);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(ResourceMessagesException.VALIDATION_LOGIN_INVALID);
        });
    }

    private LoginWithEmailAndPasswordUseCase CreateUseCase(string? password = null, WestmarchBook.Domain.Entities.User? user = null)
    {
        var userReadRepository = new IUserReadOnlyRepositoryBuilder();
        var passwordHasher = new IPasswordHasherBuilder();

        if (user is not null)
        {
            userReadRepository.GetByEmail(user);
        }
        if (password is not null and not "")
        {
            passwordHasher.VerifyPassword(password);
        }

        return new LoginWithEmailAndPasswordUseCase(userReadRepository.Build(), passwordHasher.Build());
    }
}
