using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WestmarchBook.Api.Converters;
using WestmarchBook.Api.Filters;
using WestmarchBook.Application;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Infrastructure;
using WestmarchBook.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new("pt-BR"), new("en") };

    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = builder.Configuration.GetValue<string>("JsonWebToken:SigningKey");

        options.TokenValidationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!)),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == string.Empty)
                {
                    context.Fail("Invalid user ID");

                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadOnlyRepository>();
                var existUser = await userRepository.ExisteActiveUserWithId(long.Parse(userId!));

                if (!existUser)
                {
                    context.Fail("User not found");
                }
            }
        };
    });

var app = builder.Build();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await ExecuteMigrations();

app.Run();

async Task ExecuteMigrations()
{
    await using var scope = app.Services.CreateAsyncScope();

    await DatabaseMigration.ExecuteMigrations(scope.ServiceProvider);
}
public partial class Program() { }