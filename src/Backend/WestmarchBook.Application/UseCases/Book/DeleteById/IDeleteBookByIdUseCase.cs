namespace WestmarchBook.Application.UseCases.Book.DeleteById;

public interface IDeleteBookByIdUseCase
{
    Task Execute(long id);
}
