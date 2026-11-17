using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private readonly IUserReadOnlyRepository _userReadRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IStorageService _storageService;

    public LoginWithEmailAndPasswordUseCase(IUserReadOnlyRepository userReadRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerato,
        IStorageService storageService)
    {
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerato;
        _storageService = storageService;
    }

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson request)
    {
        ValidateAndThrowOnFailures(request);

        var user = await _userReadRepository.GetByEmail(request.Email);

        if (user == null) throw new InvalidLoginException();

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);

        if (isPasswordValid == false) throw new InvalidLoginException();

        return new ResponseRegisterUserJson
        {
            UserName = user.UserName,
            ImageUrl = _storageService.GetProfilePictureUrl(user),
            Tokens = new ResponseTokensJson
            {
                AccessToken = _accessTokenGenerator.Generate(user)
            }
        };
    }

    private static void ValidateAndThrowOnFailures(RequestLoginJson request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
        }

        if (errors.Count > 0)
        {
            throw new ErrorOnValidationException(errors);
        }
    }
}
