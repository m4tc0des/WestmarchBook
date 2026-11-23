namespace WestmarchBook.Application.UseCases.Book.ChangeIllustration;

public interface IChangeIllustrationUseCase
{
    Task Execute(long bookId, Stream bookIllustration);
}
