using Bogus;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class BookBuilder
{
    public static Book Build(User user, Publisher? publisher = null, Author? author = null)
    {
        publisher ??= PublisherBuilder.Build();

        author ??= AuthorBuilder.Build();

        return new Faker<Book>()
            .RuleFor(b => b.Title, faker => faker.Commerce.ProductName())
            .RuleFor(b => b.Description, faker => faker.Lorem.Paragraph())
            .RuleFor(b => b.PublicationDate, faker => faker.Date.Past())
            .RuleFor(b => b.PublisherId, _ => publisher.Id)
            .RuleFor(b => b.Publisher, _ => publisher)
            .RuleFor(b => b.UserId, _ => user.Id)
            .RuleFor(b => b.Genres, _ => [Genre.Fantasy, Genre.Adventure])
            .RuleFor(b => b.Languages, _ => [Language.BrazilianPortuguese])
            .RuleFor(b => b.BookAuthors, _ => [
                new BookAuthor
                {
                    AuthorId = author.Id,
                    Author = author
                }
            ]);
    }
}
