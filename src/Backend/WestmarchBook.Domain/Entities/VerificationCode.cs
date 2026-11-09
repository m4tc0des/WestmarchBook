using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Entities;

public class VerificationCode: EntityBase
{
    public string Code { get; set; } = string.Empty;
    public VerificationCodeType Type { get; set; }
    public long UserId { get; set; }
}
