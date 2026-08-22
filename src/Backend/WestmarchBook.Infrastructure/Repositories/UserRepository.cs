using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Infrastructure.DataAccess;

namespace WestmarchBook.Infrastructure.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public UserRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;   
    }
    public async Task Add(Users user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<bool> ExisteActiveUserWithEmail(string email)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Email.Equals(email));
    }
}
