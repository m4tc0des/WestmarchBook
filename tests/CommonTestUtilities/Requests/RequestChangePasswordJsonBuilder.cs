using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int newPasswordLength = 8)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(request => request.CurrentPassword, request => "Ab1@" + request.Internet.Password(8))
            .RuleFor(request => request.NewPassword, request => "Ab1@" + request.Internet.Password(length: newPasswordLength));
    }
}
