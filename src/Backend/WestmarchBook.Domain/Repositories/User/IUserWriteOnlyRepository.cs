using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Domain.Repositories.User;

public interface IUserWriteOnlyRepository
{
    Task Add(Entities.User user);
}
