using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Application.UseCases.Author.Update;

public interface IUpdateAuthorUseCase
{
    Task Execute(long id, RequestUpdateAuthorJson request, Stream? authorIllustration = null);
}
