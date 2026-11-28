namespace WestmarchBook.Domain.Repositories.User;

public interface IUserWriteOnlyRepository
{
    Task Add(Entities.User user);
    Task DeactivateAccount(long userid);
    Task DeleteAccount(long userId);
}
