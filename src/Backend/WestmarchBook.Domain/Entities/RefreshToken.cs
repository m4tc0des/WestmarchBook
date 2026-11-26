namespace WestmarchBook.Domain.Entities;

public class RefreshToken: EntityBase
{
    public string Value { get; set; } = string.Empty;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}
