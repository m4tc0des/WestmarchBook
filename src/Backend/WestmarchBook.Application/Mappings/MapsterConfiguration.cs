using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Application.Mappings;

public static class MapsterConfiguration
{
    public static void Configure()
    {
        TypeAdapterConfig<RequestRegisterUserJson, User>
            .NewConfig()
            .Ignore(destination => destination.Password);

        TypeAdapterConfig<RequestRegisterBookJson, Book>
            .NewConfig()
            .Map(destination => destination.BookAuthors, request => request.Authors.Select(authorId => new BookAuthor
            {
                AuthorId = authorId
            }).ToList())
            .Map(destination => destination.BookCopies, request => request.RegistrationNumbers.Select(regNumber => new BookCopy
            {
                RegistrationNumber = regNumber,
                CopyStatus = CopyStatus.Available
            }).ToList());

        TypeAdapterConfig<Book, ResponseBookJson>
            .NewConfig()
            .Map(destination => destination.Publisher, source => new ResponsePublisherJson
            {
                Id = source.Publisher.Id,
                Name = source.Publisher.Name
            })
            .Map(destination => destination.Authors, source => source.BookAuthors.Select(ba => new ResponseAuthorJson
            {
                Id = ba.Author.Id,
                Name = ba.Author.Name
            }).ToList());
    }
}
