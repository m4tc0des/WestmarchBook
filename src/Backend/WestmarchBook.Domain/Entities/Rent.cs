using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Entities;

public class Rent
{
    public long Id { get; set; }
    public long ReaderId { get; set; }
    public Reader Reader { get; set; } = null!;
    public long BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;
    public DateTime RentDate { get; set; } = DateTime.UtcNow;
    public DateTime? ActualReturnDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public RentStatus RentStatus { get; set; } = RentStatus.Active;
}
