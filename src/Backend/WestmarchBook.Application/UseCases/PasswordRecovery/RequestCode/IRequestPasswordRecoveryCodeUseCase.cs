using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.PasswordRecovery.RequestCode;

public interface IRequestPasswordRecoveryCodeUseCase
{
    Task Execute(RequestPasswordRecoveryCodeJson request);
}
