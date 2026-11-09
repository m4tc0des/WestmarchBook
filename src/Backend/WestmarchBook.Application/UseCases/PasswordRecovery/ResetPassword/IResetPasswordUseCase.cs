using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.PasswordRecovery.ResetPassword;

public interface IResetPasswordUseCase
{
    Task Execute(RequestResetPasswordJson request);
}
