using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    Task Execute(RequestUpdateUserJson request);
}
