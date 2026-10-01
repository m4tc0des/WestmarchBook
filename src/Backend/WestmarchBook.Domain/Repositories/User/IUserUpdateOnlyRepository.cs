namespace WestmarchBook.Domain.Repositories.User;

public interface IUserUpdateOnlyRepository
{
    void UpdateProfile(Entities.User user);
    Task UpdatePassword(long userId, string passwordHash);
}
