using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Readers.GetAll;

public interface IGetAllReadersUseCase
{
    Task<IList<ResponseReaderJson>> Execute();
    Task<long> GetNextReaderId();
}
