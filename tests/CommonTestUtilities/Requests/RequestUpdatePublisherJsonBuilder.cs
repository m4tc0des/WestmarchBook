using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdatePublisherJsonBuilder
{
    public static RequestUpdatePublisherJson Build()
    {
        return new Faker<RequestUpdatePublisherJson>()
            .RuleFor(publisher => publisher.Name, faker => faker.Company.CompanyName());
    }
}
