using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateAuthorJsonBuilder
{
    public static RequestUpdateAuthorJson Build()
    {
        return new Faker<RequestUpdateAuthorJson>()
            .RuleFor(author => author.Name, faker => faker.Person.FullName);
    }
}
