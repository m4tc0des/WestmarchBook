using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using WestmarchBook.Infrastructure.DataAccess;

namespace WebApi.Tests;

public abstract class BaseIntegrationTest: IClassFixture<WestmarchBookApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly HttpClient _httpClient;
    internal readonly WestmarchBookDbContext DbContext;

    public BaseIntegrationTest(WestmarchBookApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
        _scope = factory.Services.CreateAsyncScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<WestmarchBookDbContext>();
    }

    protected async Task<HttpResponseMessage> Post(string requestUri, object request, string culture = "pt-BR")
    {
        ChangeRequestCulture(culture);

        return await _httpClient.PostAsJsonAsync(requestUri, request);
    }

    private void ChangeRequestCulture(string culture)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}
