using Bogus;
using CommonTestUtilities.Security;
using WestmarchBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var (password, passwordHashed) = GenerateRandomPassword();
        var user = new Faker<User>()
        .RuleFor(user => user.UserName, faker => faker.Person.FirstName)
        .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.UserName))
        .RuleFor(user => user.Password, _ => passwordHashed);

        return (user, password);
    }

    private static (string password, string passwordHashed) GenerateRandomPassword()
    {
        var passwordEncripter = new IPasswordHasherBuilder().Build();
        var password = "Ab1@" + new Faker().Internet.Password(8);

        return (password, passwordEncripter.HashPassword(password));
    }
}
