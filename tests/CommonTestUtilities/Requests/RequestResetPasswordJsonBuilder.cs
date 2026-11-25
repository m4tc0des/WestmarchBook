using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestResetPasswordJsonBuilder
{
    public static RequestResetPasswordJson Build(int? passwordLength = null)
    {
        return new Faker<RequestResetPasswordJson>()

            .RuleFor(request => request.Email, faker => faker.Internet.Email())
            .RuleFor(request => request.Code, faker => faker.Random.Int(1, 999_999).ToString("D6"))
            .RuleFor(request => request.NewPassword, faker =>
            {
                if (passwordLength.HasValue)
                {
                    return faker.Internet.Password(length: passwordLength.Value);
                }

                return "Ab1@" + faker.Internet.Password(length: 8);
            });
    }
}
