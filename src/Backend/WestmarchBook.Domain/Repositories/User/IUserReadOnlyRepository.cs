namespace WestmarchBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ExisteActiveUserWithEmail(string email);
    public Task<Entities.User?> GetByEmail(string email);
}
