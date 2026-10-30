using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Publisher.GetAll;

public interface IGetAllPublishersUseCase
{
    Task<IList<ResponsePublisherJson>> Execute();
}
