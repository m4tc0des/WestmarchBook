namespace WestmarchBook.Application.UseCases.DeleteAccount;

public interface IDeleteUserAccountPermanentlyUseCase
{
    Task Execute(long userId);
}
