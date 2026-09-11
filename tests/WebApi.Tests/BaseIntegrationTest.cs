using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WestmarchBook.Infrastructure.DataAccess;

namespace WebApi.Tests;

public abstract class BaseIntegrationTest : IClassFixture<WestmarchBookApplicationFactory>, IDisposable
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

    protected async Task<HttpResponseMessage> Get(string requestUri, string accessToken, string culture = "pt-BR")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(accessToken);

        return await _httpClient.GetAsync(requestUri);
    }

    protected async Task<HttpResponseMessage> Post(string requestUri, object request, string accessToken = "", string culture = "pt-BR")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(accessToken);

        return await _httpClient.PostAsJsonAsync(requestUri, request);
    }

    private void ChangeRequestCulture(string culture)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
    }

    private void AuthorizeRequest(string accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}
