using Bogus;
using WestmarchBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class PublisherBuilder
{
    public static Publisher Build()
    {
        return new Faker<Publisher>()
            .RuleFor(publisher => publisher.Name, faker => faker.Company.CompanyName());
    }
}
