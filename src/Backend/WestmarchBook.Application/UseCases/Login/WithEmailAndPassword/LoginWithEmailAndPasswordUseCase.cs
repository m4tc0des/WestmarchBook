using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.RefreshToken;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private readonly IUserReadOnlyRepository _userReadRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IStorageService _storageService;
    private readonly IRefreshTokenWriteOnlyRepository _refreshTokenWriteOnlyRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginWithEmailAndPasswordUseCase(IUserReadOnlyRepository userReadRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerato,
        IStorageService storageService,
        IRefreshTokenWriteOnlyRepository refreshTokenWriteOnlyRepository,
        IRefreshTokenGenerator refreshTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerato;
        _storageService = storageService;
        _refreshTokenWriteOnlyRepository = refreshTokenWriteOnlyRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson request)
    {
        ValidateAndThrowOnFailures(request);

        var user = await _userReadRepository.GetByEmail(request.Email);

        if (user == null) throw new InvalidLoginException();

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);

        if (isPasswordValid == false) throw new InvalidLoginException();

        var refreshToken = await GenerateAndSaveRefreshToken(user.Id);

        await _unitOfWork.Commit();

        return new ResponseRegisterUserJson
        {
            UserName = user.UserName,
            ImageUrl = user.HasImage ? _storageService.GetProfilePictureUrl(user.Id) : string.Empty,
            Tokens = new ResponseTokensJson
            {
                AccessToken = _accessTokenGenerator.Generate(user),
                RefreshToken = refreshToken
            }
        };
    }

    private async Task<string> GenerateAndSaveRefreshToken(long userId)
    {
        var refreshToken = new RefreshToken
        {
            Value = _refreshTokenGenerator.Generate(),
            UserId = userId
        };

        await _refreshTokenWriteOnlyRepository.Replace(refreshToken);

        return refreshToken.Value;
    }

    private static void ValidateAndThrowOnFailures(RequestLoginJson request)
    {
        var validator = new LoginWithEmailAndPasswordValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
