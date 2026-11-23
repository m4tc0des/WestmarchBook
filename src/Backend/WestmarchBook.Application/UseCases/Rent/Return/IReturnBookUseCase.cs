namespace WestmarchBook.Application.UseCases.Rent.Return;

public interface IReturnBookUseCase
{
    Task Execute(long rentId);
}
