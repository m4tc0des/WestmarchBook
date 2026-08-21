using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.User.Register;

public interface IRegisterUserAccountUseCase
{
    void Execute(RequestRegisterUserJson request);
}
