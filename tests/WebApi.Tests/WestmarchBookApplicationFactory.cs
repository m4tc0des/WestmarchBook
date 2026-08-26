using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MySql;

namespace WebApi.Tests;

public class WestmarchBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;

    public WestmarchBookApplicationFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0").WithDatabase("westmarchbook").Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests")
            .ConfigureAppConfiguration((_, options) =>
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DbConnection"] = _mySqlContainer.GetConnectionString()
                };

                options.AddInMemoryCollection(parameters);
            });
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _mySqlContainer.StopAsync();
    }
}
