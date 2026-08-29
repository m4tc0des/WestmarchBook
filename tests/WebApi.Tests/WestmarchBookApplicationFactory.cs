using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;
using WebApi.Tests.Resources;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Infrastructure.DataAccess;

namespace WebApi.Tests;

public class WestmarchBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentityManager User_1 { get; private set; }
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

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WestmarchBookDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var (user, password) = UserBuilder.Build();

        user.Password = passwordHasher.HashPassword(password);

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User_1 = new UserIdentityManager(user, password);
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _mySqlContainer.StopAsync();
    }
}
