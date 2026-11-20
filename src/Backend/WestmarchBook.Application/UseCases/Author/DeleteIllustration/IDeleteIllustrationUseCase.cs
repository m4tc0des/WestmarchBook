namespace WestmarchBook.Application.UseCases.Author.DeleteIllustration;

public interface IDeleteIllustrationUseCase
{
    Task Execute(long authorId);
}
