using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.RefreshToken;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Api.Token.UseRefreshToken;

public class UseRefreshTokenUseCase: IUseRefreshTokenUseCase
{
    private const int ExpirationTimeInDays = 15;
    private readonly IRefreshTokenWriteOnlyRepository _refreshTokenWriteOnlyRepository;
    private readonly IRefreshTokenReadOnlyRepository _refreshTokenReadOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public UseRefreshTokenUseCase(IRefreshTokenWriteOnlyRepository refreshTokenWriteOnlyRepository,
        IRefreshTokenReadOnlyRepository refreshTokenReadOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenWriteOnlyRepository = refreshTokenWriteOnlyRepository;
        _refreshTokenReadOnlyRepository = refreshTokenReadOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseTokensJson> Execute(RequestNewTokenJson request)
    {
        var refreshToken = await _refreshTokenReadOnlyRepository.Get(request.RefreshToken);

        if (refreshToken is null) throw new RefreshTokenExpiredException();

        var isValid = refreshToken.CreatedAt.AddDays(ExpirationTimeInDays) >= DateTime.UtcNow;

        if (isValid == false) throw new RefreshTokenExpiredException();

        var user = await _userReadOnlyRepository.GetById(refreshToken.UserId);

        var newRefreshToken = new RefreshToken
        {
            Value = _refreshTokenGenerator.Generate(),
            UserId = refreshToken.UserId
        };

        await _refreshTokenWriteOnlyRepository.Replace(newRefreshToken);

        await _unitOfWork.Commit();

        return new ResponseTokensJson
        {
            AccessToken = _accessTokenGenerator.Generate(user!),
            RefreshToken = newRefreshToken.Value
        };
    }
}
