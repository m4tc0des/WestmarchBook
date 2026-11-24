using Moq;
using WestmarchBook.Domain.Repositories.VerificationCode;

namespace CommonTestUtilities.Repositories;

public static class IVerificationCodeWriteOnlyRepositoryBuilder
{
    public static IVerificationCodeWriteOnlyRepository Build()
    {
        var mock = new Mock<IVerificationCodeWriteOnlyRepository>();

        return mock.Object;
    }
}
