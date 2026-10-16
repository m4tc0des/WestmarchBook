using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Author;
using WestmarchBook.Domain.Repositories.Book;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Infrastructure.DataAccess;
using WestmarchBook.Infrastructure.DataAccess.Repositories;
using WestmarchBook.Infrastructure.Identity;
using WestmarchBook.Infrastructure.Security.PasswordHashing;
using WestmarchBook.Infrastructure.Security.Tokens.Access;

namespace WestmarchBook.Infrastructure;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddDbContext(configuration);
            services.AddSecurity(configuration);
            services.AddAuthentication();
        }

        private void AddRepositories()
        {
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();

            services.AddScoped<IAuthorWriteOnlyRepository, AuthorRepository>();
            services.AddScoped<IAuthorReadOnlyRepository, AuthorRepository>();

            services.AddScoped<IBookWriteOnlyRepository, BookRepository>();
            services.AddScoped<IBookReadOnlyRepository, BookRepository>();

            services.AddScoped<IPublisherWriteOnlyRepository, PublisherRepository>();
            services.AddScoped<IPublisherReadOnlyRepository, PublisherRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private void AddSecurity(IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IAccessTokenGenerator>(provider =>
            {
                var expirationTimeInMinutes = configuration.GetValue<uint>("JsonWebToken:ExpirationTimeInMinutes");
                var signingKey = configuration.GetValue<string>("JsonWebToken:SigningKey");

                return new JwtTokenHandler(expirationTimeInMinutes, signingKey!);
            });
        }

        private void AddAuthentication()
        {
            services.AddScoped<ILoggedUser, LoggedUser>();
        }

        private void AddDbContext(IConfiguration configuration)
        {
            services.AddDbContext<WestmarchBookDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection");
                options.UseMySQL(connectionString!);
            });
        }
    }
}
