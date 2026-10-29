using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Application.UseCases.Book.GetAll;

public interface IGetAllBooksUseCase
{
    Task<IList<ResponseBookJson>> Execute();
    Task<long> GetNextBookId();
}
