using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Publisher.GetById;

public interface IGetPublisherByIdUseCase
{
    Task<ResponsePublisherJson> Execute(long id);
}
