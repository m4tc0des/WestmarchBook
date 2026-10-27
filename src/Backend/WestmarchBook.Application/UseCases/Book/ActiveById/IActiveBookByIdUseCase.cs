namespace WestmarchBook.Application.UseCases.Book.ActiveById;

public interface IActiveBookByIdUseCase
{
    Task Execute(long id);
}
