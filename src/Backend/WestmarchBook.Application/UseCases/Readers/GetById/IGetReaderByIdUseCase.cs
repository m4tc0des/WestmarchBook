using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Readers.GetById;

public interface IGetReaderByIdUseCase
{
    Task<ResponseReaderJson> Execute(long id);
}
