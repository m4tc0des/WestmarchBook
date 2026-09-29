using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build()
    {
        return new Faker<RequestRegisterUserJson>()
            .RuleFor(request => request.UserName, faker => faker.Person.FirstName)
            .RuleFor(request => request.Email, (faker, x) => faker.Internet.Email(x.Name))
            .RuleFor(request => request.Password, faker => "Ab1@" + faker.Internet.Password(8));
    }
}
