namespace WestmarchBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ExisteActiveUserWithEmail(string email);
    Task<Entities.User?> GetById(long id);
    Task<Entities.User?> GetByIdForDeletion(long id);
    public Task<Entities.User?> GetByEmail(string email);

}
