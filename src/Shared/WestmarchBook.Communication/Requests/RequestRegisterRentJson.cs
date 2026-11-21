using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Communication.Requests;

public class RequestRegisterRentJson
{
    public long ReaderId { get; set; }
    public long BookCopyId { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
}
