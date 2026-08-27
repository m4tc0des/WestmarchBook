using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private readonly IUserReadOnlyRepository _userReadRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginWithEmailAndPasswordUseCase(IUserReadOnlyRepository userReadRepository, IPasswordHasher passwordHasher)
    {
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson request)
    {
        var user = await _userReadRepository.GetByEmail(request.Email);

        if (user == null)
        {
            throw new InvalidLoginException();
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);

        if (isPasswordValid == false)
        {
            throw new InvalidLoginException();
        }

        return new ResponseRegisterUserJson
        {
            Name = user.Name
        };
    }
}
