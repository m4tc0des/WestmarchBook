using Bogus;
using WestmarchBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class AuthorBuilder
{
    public static Author Build()
    {
        return new Faker<Author>()
            .RuleFor(author => author.Name, faker => faker.Person.FullName);
    }
}
