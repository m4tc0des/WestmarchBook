using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using Moq;
using Shouldly;
using WestmarchBook.Application.UseCases.DeleteAccount;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Services.StorageService;

namespace UseCases.Tests.DeleteAccount;

public class DeleteUserAccountPermanentlyUseCaseTests
{
    [Fact]
    public async Task Success_When_UserHasProfilePicture()
    {
        var (user, _) = UserBuilder.Build();
        user.Id = 42;
        user.Active = false;
        user.HasImage = true;

        var userReadRepository = new IUserReadOnlyRepositoryBuilder().GetByIdForDeletion(user).Build();
        var userWriteRepositoryBuilder = new IUserWriteOnlyRepositoryBuilder();
        var storageBuilder = new IStorageServiceBuilder();
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = new DeleteUserAccountPermanentlyUseCase(
            userReadRepository,
            userWriteRepositoryBuilder.BuildInstance(),
            storageBuilder.BuildInstance(),
            unitOfWorkBuilder.BuildInstance());

        await useCase.Execute(user.Id).ShouldNotThrowAsync();

        storageBuilder.VerifyDeleteUserFilesCalled(user, 1);
        storageBuilder.VerifyDeleteBookIllustrationCalled(0);
        storageBuilder.VerifyDeleteAuthorIllustrationCalled(0);
        userWriteRepositoryBuilder.VerifyDeleteAccountCalled(user.Id, 1);
        unitOfWorkBuilder.VerifyCommitCalled(1);
    }

    [Fact]
    public async Task Success_When_UserHasNoProfilePicture()
    {
        var (user, _) = UserBuilder.Build();
        user.Id = 43;
        user.Active = false;
        user.HasImage = false;

        var userReadRepository = new IUserReadOnlyRepositoryBuilder().GetByIdForDeletion(user).Build();
        var userWriteRepositoryBuilder = new IUserWriteOnlyRepositoryBuilder();
        var storageBuilder = new IStorageServiceBuilder();
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = new DeleteUserAccountPermanentlyUseCase(
            userReadRepository,
            userWriteRepositoryBuilder.BuildInstance(),
            storageBuilder.BuildInstance(),
            unitOfWorkBuilder.BuildInstance());

        await useCase.Execute(user.Id).ShouldNotThrowAsync();

        storageBuilder.VerifyDeleteUserFilesCalled(0);
        storageBuilder.VerifyDeleteBookIllustrationCalled(0);
        storageBuilder.VerifyDeleteAuthorIllustrationCalled(0);
        userWriteRepositoryBuilder.VerifyDeleteAccountCalled(user.Id, 1);
        unitOfWorkBuilder.VerifyCommitCalled(1);
    }

    [Fact]
    public async Task Success_When_UserNotFoundForDeletion()
    {
        var userReadRepository = new IUserReadOnlyRepositoryBuilder().Build();
        var userWriteRepositoryBuilder = new IUserWriteOnlyRepositoryBuilder();
        var storageBuilder = new IStorageServiceBuilder();
        var unitOfWorkBuilder = new IUnitOfWorkBuilder();

        var useCase = new DeleteUserAccountPermanentlyUseCase(
            userReadRepository,
            userWriteRepositoryBuilder.BuildInstance(),
            storageBuilder.BuildInstance(),
            unitOfWorkBuilder.BuildInstance());

        await useCase.Execute(999).ShouldNotThrowAsync();

        storageBuilder.VerifyDeleteUserFilesCalled(0);
        userWriteRepositoryBuilder.VerifyDeleteAccountCalled(999, 0);
        unitOfWorkBuilder.VerifyCommitCalled(0);
    }

    [Fact]
    public async Task Success_ShouldExecuteStepsInExpectedOrder()
    {
        var (user, _) = UserBuilder.Build();
        user.Id = 77;
        user.Active = false;
        user.HasImage = true;

        var callOrder = new List<string>();

        var userReadMock = new Mock<IUserReadOnlyRepository>();
        userReadMock
            .Setup(repo => repo.GetByIdForDeletion(user.Id))
            .Callback(() => callOrder.Add("GetByIdForDeletion"))
            .ReturnsAsync(user);

        var storageMock = new Mock<IStorageService>();
        storageMock
            .Setup(storage => storage.DeleteUserFiles(user))
            .Callback(() => callOrder.Add("DeleteUserFiles"))
            .Returns(Task.CompletedTask);

        var userWriteMock = new Mock<IUserWriteOnlyRepository>();
        userWriteMock
            .Setup(repo => repo.DeleteAccount(user.Id))
            .Callback(() => callOrder.Add("DeleteAccount"))
            .Returns(Task.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(uow => uow.Commit())
            .Callback(() => callOrder.Add("Commit"))
            .Returns(Task.CompletedTask);

        var useCase = new DeleteUserAccountPermanentlyUseCase(
            userReadMock.Object,
            userWriteMock.Object,
            storageMock.Object,
            unitOfWorkMock.Object);

        await useCase.Execute(user.Id);

        callOrder.ShouldBe(["GetByIdForDeletion", "DeleteUserFiles", "DeleteAccount", "Commit"]);
    }
}
