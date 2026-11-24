using Bogus;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class RentBuilder
{
    public static Rent Build(
        Reader? reader = null,
        BookCopy? bookCopy = null,
        RentStatus rentStatus = RentStatus.Active)
    {
        reader ??= ReaderBuilder.Build();
        bookCopy ??= BookCopyBuilder.Build(copyStatus: CopyStatus.Rented);

        return new Faker<Rent>()
            .RuleFor(r => r.Id, faker => faker.Random.Long(1, 1000))
            .RuleFor(r => r.ReaderId, _ => reader.Id)
            .RuleFor(r => r.Reader, _ => reader)
            .RuleFor(r => r.BookCopyId, _ => bookCopy.Id)
            .RuleFor(r => r.BookCopy, _ => bookCopy)
            .RuleFor(r => r.RentDate, _ => DateTime.UtcNow.AddDays(-2))
            .RuleFor(r => r.ExpectedReturnDate, _ => DateTime.UtcNow.AddDays(5))
            .RuleFor(r => r.ActualReturnDate, _ => rentStatus == RentStatus.Finished ? DateTime.UtcNow : null)
            .RuleFor(r => r.RentStatus, _ => rentStatus);
    }
}
