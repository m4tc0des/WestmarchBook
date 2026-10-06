using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.User.Update;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.User.Update;

public class UpdateUserUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();

        user.UserName.ShouldBe(request.UserName);
        user.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_NameIsEmpty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        request.UserName = string.Empty;

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(exception =>
        {
            exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
            exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
            {
                errors.Count().ShouldBe(1);
                errors.ShouldContain(ResourceMessagesException.VALIDATION_USERNAME_REQUIRED);
            });
        });
        user.UserName.ShouldNotBe(request.UserName);
        user.Email.ShouldNotBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_EmailAlreadyExists()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, request.Email);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(exception =>
        {
            exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
            exception.GetErrorMessage().ShouldSatisfyAllConditions(errors =>
            {
                errors.Count().ShouldBe(1);
                errors.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS);
            });
        });

        user.UserName.ShouldNotBe(request.UserName);
        user.Email.ShouldNotBe(request.Email);
    }

    private static UpdateUserUseCase CreateUseCase(WestmarchBook.Domain.Entities.User user, string? emailAlreadyExists = null)
    {
        var userUpdateRepository = IUserUpdateOnlyRepositoryBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userReadRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();

        if (string.IsNullOrEmpty(emailAlreadyExists) == false)
        {
            userReadRepositoryBuilder.ExistActiveUserWithEmail(emailAlreadyExists);
        }

        return new UpdateUserUseCase(loggedUser, userReadRepositoryBuilder.Build(), userUpdateRepository, unitOfWork);
    }
}
