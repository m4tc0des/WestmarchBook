using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public static class RequestPasswordRecoveryJsonBuilder
{
    public static RequestPasswordRecoveryCodeJson Build()
    {
        return new Faker<RequestPasswordRecoveryCodeJson>()
            .RuleFor(request => request.Email, f => f.Internet.Email());
    }
}
