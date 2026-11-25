using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using WestmarchBook.Application.UseCases.PasswordRecovery.RequestCode;
using WestmarchBook.Communication.Requests;

namespace UseCases.Tests.PasswordRecovery.RequestCode;

public class RequestPasswordRecoveryCodeUseCaseTests
{
    [Fact]
    public async Task Success_WhenUserExists()
    {
        var (user, _) = UserBuilder.Build();

        var request = new RequestPasswordRecoveryCodeJson
        {
            Email = user.Email
        };

        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Success_WhenUserDoesNotExist()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();

        var useCase = CreateUseCase();

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    private static RequestPasswordRecoveryCodeUseCase CreateUseCase(WestmarchBook.Domain.Entities.User? user = null)
    {
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var verificationCodeWriteOnlyRepository = IVerificationCodeWriteOnlyRepositoryBuilder.Build();
        var emailService = ISendEmailServiceBuilder.Build();
        var userReadOnlyRepository = new IUserReadOnlyRepositoryBuilder();

        if (user is not null)
        {
            userReadOnlyRepository.GetByEmail(user);
        }

        return new RequestPasswordRecoveryCodeUseCase(userReadOnlyRepository.Build(), verificationCodeWriteOnlyRepository, emailService, unitOfWork);
    }
}
