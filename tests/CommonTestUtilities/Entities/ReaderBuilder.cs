using Bogus;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class ReaderBuilder
{
    public static Reader Build()
    {
        return new Faker<Reader>()
            .RuleFor(r => r.FullName, faker => faker.Person.FullName)
            .RuleFor(r => r.Email, faker => faker.Internet.Email())
            .RuleFor(r => r.Phone, faker => faker.Phone.PhoneNumber("119########"))
            .RuleFor(r => r.DocumentNumber, _ => GenerateValidCpf())
            .RuleFor(r => r.City, _ => "Salvador")
            .RuleFor(r => r.Status, _ => ReaderStatus.Good);
    }

    public static string GenerateValidCpf()
    {
        var random = new Random();
        var numbers = new int[9];
        for (int i = 0; i < 9; i++)
            numbers[i] = random.Next(0, 10);

        if (numbers.Distinct().Count() == 1)
            numbers[8] = (numbers[8] + 1) % 10;

        int sum1 = 0;
        for (int i = 0; i < 9; i++)
            sum1 += numbers[i] * (10 - i);
        int rem1 = sum1 % 11;
        int digit1 = rem1 < 2 ? 0 : 11 - rem1;

        int sum2 = 0;
        for (int i = 0; i < 9; i++)
            sum2 += numbers[i] * (11 - i);
        sum2 += digit1 * 2;
        int rem2 = sum2 % 11;
        int digit2 = rem2 < 2 ? 0 : 11 - rem2;

        return $"{string.Concat(numbers)}{digit1}{digit2}";
    }
}
