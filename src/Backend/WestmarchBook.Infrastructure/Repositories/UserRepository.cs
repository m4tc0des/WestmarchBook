using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Infrastructure.DataAccess;

namespace WestmarchBook.Infrastructure.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public UserRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<bool> ExisteActiveUserWithEmail(string email)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Email.Equals(email));
    }

    public async Task<bool> ExisteActiveUserWithId(long id)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Id == id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Active && user.Email.Equals(email));
    }

    public async Task UpdatePassword(long userId, string passwordHash)
    {
        await _dbContext.Users.Where(user => user.Id == userId).ExecuteUpdateAsync( options => options.SetProperty(user => user.Password, passwordHash));
    }

    public void UpdateProfile(User user)
    {
        _dbContext.Users.Attach(user);

        _dbContext.Entry(user).Property(user => user.UserName).IsModified = true;
        _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
    }
}
