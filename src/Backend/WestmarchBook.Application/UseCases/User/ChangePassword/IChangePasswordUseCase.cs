using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.User.ChangePassword;

public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson request);
}
