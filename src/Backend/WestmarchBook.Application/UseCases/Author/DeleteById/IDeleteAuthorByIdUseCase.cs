namespace WestmarchBook.Application.UseCases.Author.DeleteById;

public interface IDeleteAuthorByIdUseCase
{
    Task Execute(long id);
}
