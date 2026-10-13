using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Application.Mappings;

internal static class MapsterConfiguration
{
    internal static void Configure()
    {
        TypeAdapterConfig<RequestRegisterUserJson, User>
            .NewConfig()
            .Ignore(destination => destination.Password);

        TypeAdapterConfig<RequestRegisterBookJson, Book>
            .NewConfig()
            .Map(destination => destination.BookAuthors, request => request.Authors.Select(authorId => new BookAuthor
            {
                AuthorId = authorId
            }).ToList());
    }
}
