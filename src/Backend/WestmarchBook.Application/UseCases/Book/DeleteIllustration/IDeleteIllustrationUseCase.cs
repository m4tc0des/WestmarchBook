namespace WestmarchBook.Application.UseCases.Book.DeleteIllustration;

public interface IDeleteIllustrationUseCase
{
    Task Execute(long bookId);
}
