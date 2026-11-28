using CommonTestUtilities.Repositories;
using Shouldly;
using WestmarchBook.Application.UseCases.Login.Logout;

namespace UseCases.Tests.Login.Logout;

public class LogoutUseCaseTests
{
    [Fact]
    public async Task Success_When_RefreshTokenIsProvided()
    {
        var repoBuilder = new IRefreshTokenWriteOnlyRepositoryBuilder();
        var useCase = new LogoutUseCase(repoBuilder.BuildInstance());

        await useCase.Execute("valid-refresh-token").ShouldNotThrowAsync();

        repoBuilder.VerifyDeleteByTokenCalled("valid-refresh-token", 1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Success_ShouldSkipRepository_When_RefreshTokenIsEmpty(string refreshToken)
    {
        var repoBuilder = new IRefreshTokenWriteOnlyRepositoryBuilder();
        var useCase = new LogoutUseCase(repoBuilder.BuildInstance());

        await useCase.Execute(refreshToken).ShouldNotThrowAsync();

        repoBuilder.VerifyDeleteByTokenCalled(refreshToken, 0);
    }
}
