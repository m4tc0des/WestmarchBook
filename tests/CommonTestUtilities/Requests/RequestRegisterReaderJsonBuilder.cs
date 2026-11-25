using Bogus;
using CommonTestUtilities.Entities;
using WestmarchBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterReaderJsonBuilder
{
    public static RequestRegisterReaderJson Build()
    {
        return new Faker<RequestRegisterReaderJson>()
            .RuleFor(reader => reader.FullName, faker => faker.Person.FullName)
            .RuleFor(reader => reader.Email, faker => faker.Internet.Email())
            .RuleFor(reader => reader.Phone, faker => faker.Phone.PhoneNumber("119########"))
            .RuleFor(reader => reader.DocumentNumber, _ => ReaderBuilder.GenerateValidCpf())
            .RuleFor(reader => reader.City, _ => "Salvador");
    }
}
