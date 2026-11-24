using Moq;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.VerificationCode;

namespace CommonTestUtilities.Repositories;

public class IVerificationCodeReadOnlyRepositoryBuilder
{
    private readonly Mock<IVerificationCodeReadOnlyRepository> _mock = new();

    public IVerificationCodeReadOnlyRepositoryBuilder GetCode(VerificationCode verificationCode)
    {
        _mock.Setup(repository => repository.GetCode(verificationCode.UserId, verificationCode.Code, verificationCode.Type))
            .ReturnsAsync(verificationCode);
        
        return this;
    }

    public IVerificationCodeReadOnlyRepositoryBuilder GetExternalLoginCode(VerificationCode verificationCode)
    {
        _mock.Setup(repository => repository.GetExternalLoginCode(verificationCode.Code))
            .ReturnsAsync(verificationCode);

        return this;
    }

    public IVerificationCodeReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
