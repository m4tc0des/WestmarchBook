using Bogus;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class BookBuilder
{
    public static Book Build(Publisher? publisher = null, Author? author = null)
    {
        publisher ??= PublisherBuilder.Build();

        author ??= AuthorBuilder.Build();

        return new Faker<Book>()
            .RuleFor(b => b.Title, faker => faker.Commerce.ProductName())
            .RuleFor(b => b.Description, faker => faker.Lorem.Paragraph())
            .RuleFor(b => b.PublicationDate, faker => faker.Date.Past())
            .RuleFor(b => b.PublisherId, _ => publisher.Id)
            .RuleFor(b => b.Publisher, _ => publisher)
            .RuleFor(b => b.Genres, _ => [Genre.Fantasy, Genre.Adventure])
            .RuleFor(b => b.BookAuthors, _ => [
                new BookAuthor
                {
                    AuthorId = author.Id,
                    Author = author
                }
            ])
            .RuleFor(b => b.BookCopies, faker => [
                new BookCopy
                {
                    RegistrationNumber = faker.Random.AlphaNumeric(8),
                    CopyStatus = CopyStatus.Available
                }
            ]);
    }
}
