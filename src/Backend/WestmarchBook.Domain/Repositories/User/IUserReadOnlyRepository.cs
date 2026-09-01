namespace WestmarchBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ExisteActiveUserWithEmail(string email);
    Task<bool> ExisteActiveUserWithId(long id);
    public Task<Entities.User?> GetByEmail(string email);

}
