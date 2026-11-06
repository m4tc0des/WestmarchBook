using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Entities;

public class BookCopy
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public CopyStatus CopyStatus { get; set; } = CopyStatus.Available;
}
