using Bogus;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class BookCopyBuilder
{
    public static BookCopy Build(Book? book = null, CopyStatus copyStatus = CopyStatus.Available)
    {
        book ??= BookBuilder.Build();

        return new Faker<BookCopy>()
            .RuleFor(c => c.Id, faker => faker.Random.Long(1, 1000))
            .RuleFor(c => c.BookId, _ => book.Id)
            .RuleFor(c => c.Book, _ => book)
            .RuleFor(c => c.RegistrationNumber, faker => faker.Random.AlphaNumeric(8))
            .RuleFor(c => c.CopyStatus, _ => copyStatus);
    }
}
