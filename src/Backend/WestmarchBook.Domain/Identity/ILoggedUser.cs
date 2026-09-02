using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Domain.Identity;

public interface ILoggedUser
{
    Task<User> GetProfile();
    long GetUserId();
}
