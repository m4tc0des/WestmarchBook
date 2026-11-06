using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Entities;

public class Reader : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public ReaderStatus Status { get; set; } = ReaderStatus.Good;
}
