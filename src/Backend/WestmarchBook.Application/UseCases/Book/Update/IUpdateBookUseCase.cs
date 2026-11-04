using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.Book.Update;

public interface IUpdateBookUseCase
{
    Task Execute(long id, RequestUpdateBookJson request);
}
