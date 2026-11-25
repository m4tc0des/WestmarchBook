using Bogus;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterRentJsonBuilder
{
    public static RequestRegisterRentJson Build()
    {
        return new Faker<RequestRegisterRentJson>()
            .RuleFor(r => r.ReaderId, faker => faker.Random.Long(1, 1000))
            .RuleFor(r => r.BookCopyId, faker => faker.Random.Long(1, 1000))
            .RuleFor(r => r.ExpectedReturnDate, _ => DateTime.UtcNow.Date.AddDays(7));
    }
}
