namespace WebApi.Tests.Resources;

public class UserIdentityManager
{
    private readonly WestmarchBook.Domain.Entities.User _user;
    private readonly string _password;

    public UserIdentityManager(WestmarchBook.Domain.Entities.User user, string password)
    {
        _user = user;   
        _password = password;
    }

    public long GetId()
    {
        return _user.Id;
    }

    public string GetName()
    {
        return _user.Name;
    }

    public string GetEmail()
    {
        return _user.Email;
    }

    public string GetPassword()
    {
        return _password;
    }
}
