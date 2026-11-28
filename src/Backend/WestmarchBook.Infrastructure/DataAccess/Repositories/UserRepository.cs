using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.User;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public UserRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(User user)
    {
        await _dbContext
            .Users
            .AddAsync(user);
    }

    public async Task<bool> ExisteActiveUserWithEmail(string email)
    {
        return
            await _dbContext
            .Users
            .AnyAsync(user => user.Active && user.Email.Equals(email));
    }

    public async Task<User?> GetById(long id)
    {
        return
            await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Active && user.Id == id);
    }

    public async Task<User?> GetByIdForDeletion(long id)
    {
        return
            await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => !user.Active && user.Id == id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return
            await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Active && user.Email == email);
    }

    public async Task UpdatePassword(long userId, string passwordHash)
    {
        await _dbContext
            .Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(options => options.SetProperty(user => user.Password, passwordHash));
    }

    public void UpdateProfile(User user)
    {
        _dbContext.Users.Attach(user);

        _dbContext.Entry(user).Property(user => user.UserName).IsModified = true;
        _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
    }

    public async Task UpdateProfilePictureStatus(long userId, bool hasProfilePicture)
    {
        await _dbContext
            .Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.HasImage, hasProfilePicture));
    }

    public async Task DeactivateAccount(long userid)
    {
        await _dbContext
            .Users
            .Where(user => user.Active && user.Id == userid)
            .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.Active, false));
    }

    public async Task DeleteAccount(long userId)
    {
        var userToDelete =
            await _dbContext
            .Users
            .FirstOrDefaultAsync(user => user.Active == false && user.Id == userId);

        if (userToDelete is not null)
        {
            var verificationCodes =
                await _dbContext
                .VerificationCodes
                .Where(verificationCode => verificationCode.UserId == userId)
                .ToListAsync();
            _dbContext.VerificationCodes.RemoveRange(verificationCodes);

            var refreshTokens = await _dbContext.RefreshTokens
                .Where(token => token.UserId == userId)
                .ToListAsync();
            _dbContext.RefreshTokens.RemoveRange(refreshTokens);

            _dbContext.Users.Remove(userToDelete);
        }
    }
}
