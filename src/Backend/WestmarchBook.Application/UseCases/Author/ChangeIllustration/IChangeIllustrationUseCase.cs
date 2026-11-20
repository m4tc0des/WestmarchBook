namespace WestmarchBook.Application.UseCases.Author.ChangeIllustration;

public interface IChangeIllustrationUseCase
{
    Task Execute(long authorId, Stream authorIllustration);
}
