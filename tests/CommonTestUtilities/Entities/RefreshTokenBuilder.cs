using Bogus;
using WestmarchBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class RefreshTokenBuilder
{
    public static RefreshToken Build(User user)
    {
        return new Faker<RefreshToken>()
            .RuleFor(token => token.Value, faker => faker.Random.String(64))
            .RuleFor(token => token.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(token => token.UserId, _ => user.Id);
    }
}
