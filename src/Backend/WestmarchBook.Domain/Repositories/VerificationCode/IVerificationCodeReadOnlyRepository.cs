namespace WestmarchBook.Domain.Repositories.VerificationCode;

public interface IVerificationCodeReadOnlyRepository
{
    Task<Entities.VerificationCode?> GetCode(long userId, string code, Enums.VerificationCodeType type); 
}
