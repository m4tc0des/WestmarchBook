using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(request => request.UserName, faker => faker.Person.UserName)
            .RuleFor(request => request.Email, (faker, x) => faker.Internet.Email(x.UserName));
    }
}
