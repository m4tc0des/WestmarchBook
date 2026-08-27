using Bogus;
using CommonTestUtilities.Security;
using WestmarchBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static User Build()
    {
        return new Faker<User>()
        .RuleFor(user => user.Name, faker => faker.Person.FirstName)
        .RuleFor(user => user.Email, faker => faker.Internet.Email())
        .RuleFor(user => user.Password, _ => GenerateRandomPassword());
    }

    private static string GenerateRandomPassword()
    {
        var passwordEncripter = new IPasswordHasherBuilder().Build();
        var password = new Faker().Internet.Password();

        return passwordEncripter.PasswordHash(password);
    }
}
