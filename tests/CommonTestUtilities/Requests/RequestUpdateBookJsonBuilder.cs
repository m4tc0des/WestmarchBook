using Bogus;
using WestmarchBook.Communication.Enums;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateBookJsonBuilder
{
    public static RequestUpdateBookJson Build()
    {
        return new Faker<RequestUpdateBookJson>()
            .RuleFor(book => book.Title, faker => faker.Commerce.ProductName())
            .RuleFor(book => book.Description, faker => faker.Lorem.Sentence(10))
            .RuleFor(book => book.PublicationDate, faker => faker.Date.Past())
            .RuleFor(book => book.PublisherId, faker => faker.Random.Long(1, 1000))
            .RuleFor(book => book.Authors, faker => [faker.Random.Long(1, 1000)])
            .RuleFor(book => book.Genres, faker => [faker.PickRandom<Genre>()])
            .RuleFor(book => book.RegistrationNumbers, faker => [faker.Random.AlphaNumeric(8)]);
    }
}
