using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.Readers.Update;

public interface IUpdateReaderUseCase
{
    Task Execute(long id, RequestUpdateReaderJson request);
}
