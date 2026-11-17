namespace WestmarchBook.Domain.Services.EmailService;

public interface ISendEmailService
{
    Task SendPasswordRecoveryCode(string toEmail, string userName, string code);
}
