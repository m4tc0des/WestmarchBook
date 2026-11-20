namespace WestmarchBook.Application.UseCases.Publisher.DeleteById;

public interface IDeletePublisherByIdUseCase
{
    Task Execute(long id);
}
