namespace WestmarchBook.Application.UseCases.Book.InactiveById;

public interface IInactiveBookByIdUseCase
{
    Task Execute(long id);
}
