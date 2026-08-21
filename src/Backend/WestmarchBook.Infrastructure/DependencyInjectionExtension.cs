using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Infrastructure.Security.PasswordHashing;

namespace WestmarchBook.Infrastructure;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure()
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        }
    }
}
