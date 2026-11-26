using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.RefreshToken;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class RefreshTokenRepository: IRefreshTokenWriteOnlyRepository, IRefreshTokenReadOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public RefreshTokenRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _dbContext
            .RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(token => token.Active && token.Value.Equals(refreshToken));
    }

    public async Task Replace(RefreshToken refreshToken)
    {
        var existingTokens = await _dbContext.RefreshTokens
            .Where(token => token.UserId == refreshToken.UserId)
            .ToListAsync();

        _dbContext.RefreshTokens.RemoveRange(existingTokens);

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }
}
