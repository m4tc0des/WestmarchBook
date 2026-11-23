using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.Publisher.Update;

public interface IUpdatePublisherUseCase
{
    Task Execute(long id, RequestUpdatePublisherJson request);
}
