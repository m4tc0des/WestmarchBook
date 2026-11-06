using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Dtos;

public record BookFilterDto()
{
    public string? SearchTerm { get; init; } = string.Empty;
    public Genre? Genre { get; init; }
    public long? PublisherId { get; set; }
    public long? AuthorId { get; set; }
}
