using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Messaging;
using CommonTestUtilities.Repositories;
using Shouldly;
using WestmarchBook.Application.UseCases.DeleteAccount;

namespace UseCases.Tests.DeleteAccount;

public class DeleteAccountUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        user.Id = 10;

        var loggedUser = ILoggedUserBuilder.Build(user);
        var userWriteRepositoryBuilder = new IUserWriteOnlyRepositoryBuilder();
        var scheduleBuilder = new IScheduleAccountDeletionBuilder();

        var useCase = new DeleteAccountUseCase(
            loggedUser,
            userWriteRepositoryBuilder.BuildInstance(),
            scheduleBuilder.BuildInstance());

        await useCase.Execute().ShouldNotThrowAsync();

        userWriteRepositoryBuilder.VerifyDeactivateAccountCalled(user.Id, 1);
        scheduleBuilder.VerifyScheduleCalled(user.Id, 1);
    }
}
