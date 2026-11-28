using WestmarchBook.Domain.Repositories.RefreshToken;

namespace WestmarchBook.Application.UseCases.Login.Logout;

public class LogoutUseCase : ILogoutUseCase
{
    private readonly IRefreshTokenWriteOnlyRepository _refreshTokenWriteOnlyRepository;

    public LogoutUseCase(IRefreshTokenWriteOnlyRepository refreshTokenWriteOnlyRepository)
    {
        _refreshTokenWriteOnlyRepository = refreshTokenWriteOnlyRepository;
    }

    public async Task Execute(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        await _refreshTokenWriteOnlyRepository.DeleteByToken(refreshToken);
    }
}
