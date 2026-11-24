using Microsoft.EntityFrameworkCore;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories.VerificationCode;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class VerificationCodeRepository : IVerificationCodeWriteOnlyRepository, IVerificationCodeReadOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public VerificationCodeRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Replace(VerificationCode verificationCode)
    {
        var existingCode = await _dbContext.VerificationCodes
            .Where(code => code.UserId == verificationCode.UserId && code.Type == verificationCode.Type)
            .ToListAsync();

        _dbContext.VerificationCodes.RemoveRange(existingCode);

        _dbContext.VerificationCodes.Add(verificationCode);
    }

    public async Task Delete(VerificationCode verificationCode)
    {
        await _dbContext.VerificationCodes
            .Where(code => code.Id == verificationCode.Id)
            .ExecuteDeleteAsync();
    }
    public async Task<VerificationCode?> GetCode(long userId, string code, VerificationCodeType type)
    {
        return await _dbContext.VerificationCodes
            .AsNoTracking()
            .SingleOrDefaultAsync(verificationCode =>
            verificationCode.UserId == userId &&
            verificationCode.Code == code &&
            verificationCode.Type == type);
    }

    public async Task<VerificationCode?> GetExternalLoginCode(string code)
    {
        return await _dbContext.VerificationCodes
            .AsNoTracking()
            .SingleOrDefaultAsync(verificationCOde =>
            verificationCOde.Code == code &&
            verificationCOde.Type == VerificationCodeType.ExternalLoginExchange);
    }
}
