namespace WestmarchBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ExisteActiveUserWithEmail(string email);
}
