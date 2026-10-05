using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.User.ChangePassword;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.User.ChangePassword;

public class ChangePasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        request.CurrentPassword = password;

        var useCase = CreateUseCase(user, password);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_NewPasswordIsEmpty()
    {
        (var user, var password) = UserBuilder.Build();

        var request = new RequestChangePasswordJson
        {
            CurrentPassword = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUseCase(user, password);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(exception =>
        {
            exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

            exception.GetErrorMessage().ShouldSatisfyAllConditions(exception =>
            {
                exception.Count.ShouldBe(1);
                exception.ShouldContain(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
            });
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_CurrentPasswordDoesNotMatch()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUseCase(user, "invalidpassword");

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(exception =>
        {
            exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
            exception.GetErrorMessage().ShouldSatisfyAllConditions(exception =>
            {
                exception.Count.ShouldBe(1);
                exception.ShouldContain(ResourceMessagesException.VALIDATION_CURRENT_PASSWORD);
            });
        });
    }

    private static ChangePasswordUseCase CreateUseCase(WestmarchBook.Domain.Entities.User user, string password)
    {
        var userUpdateRepository = IUserUpdateOnlyRepositoryBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var passwordHasher = new IPasswordHasherBuilder().VerifyPassword(password).Build();

        return new ChangePasswordUseCase(loggedUser, passwordHasher, userUpdateRepository);
    }
}
