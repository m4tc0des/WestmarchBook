using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using Shouldly;
using WestmarchBook.Application.UseCases.User.Profile;

namespace UseCases.Tests.Profile;

public class GetUserProfileUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);
        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Email.ShouldBe(user.Email);
    }

    private GetUserProfileUseCase CreateUseCase(WestmarchBook.Domain.Entities.User user)
    {
        var loggedUser = ILoggedUserBuilder.Build(user);

        return new GetUserProfileUseCase(loggedUser);
    }
}
