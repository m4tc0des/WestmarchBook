using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Infrastructure.DataAccess;

namespace WestmarchBook.Infrastructure.Identity;

internal sealed class LoggedUser : ILoggedUser
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly WestmarchBookDbContext _dbContext;
    public LoggedUser(IAccessTokenProvider accessTokenProvider, WestmarchBookDbContext dbContext)
    {
        _accessTokenProvider = accessTokenProvider;
        _dbContext = dbContext;
    }

    public async Task<User> GetProfile()
    {
       var userId = GetUserId();

        return await _dbContext.Users.FirstAsync(user => user.Active && user.Id == userId);
    }

    public long GetUserId()
    {
        var accessToken = _accessTokenProvider.GetToken();
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(accessToken);
        var subject = jsonWebToken.Claims.First(claim => claim.Type.Equals(JwtRegisteredClaimNames.Sub));

        return long.Parse(subject.Value);
    }
}
