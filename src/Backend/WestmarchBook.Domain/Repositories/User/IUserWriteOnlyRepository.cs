using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Domain.Repositories.User;

public interface IUserWriteOnlyRepository
{
    Task Add(Users user);
}
