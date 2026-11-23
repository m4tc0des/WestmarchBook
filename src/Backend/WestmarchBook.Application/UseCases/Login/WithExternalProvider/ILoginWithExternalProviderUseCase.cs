using WestmarchBook.Domain.Dtos;

namespace WestmarchBook.Application.UseCases.Login.WithExternalProvider;

public interface ILoginWithExternalProviderUseCase
{
    Task<string> Execute(ExternalLoginInfo externalLoginInfo);
}
