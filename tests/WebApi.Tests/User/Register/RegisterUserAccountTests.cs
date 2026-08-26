using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WestmarchBook.Exception;
using WestmarchBook.Infrastructure.DataAccess;

namespace WebApi.Tests.User.Register;

public class RegisterUserAccountTests : IClassFixture<WestmarchBookApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private const string REQUEST_URI = "/users";
    private readonly WestmarchBookDbContext _dbContext;

    public RegisterUserAccountTests(WestmarchBookApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
        var scope = factory.Services.CreateAsyncScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<WestmarchBookDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldBeEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeEmpty();

        var userExists = await _dbContext.Users.AnyAsync(user =>
        user.Active &&
        user.Name.Equals(request.Name) &&
        user.Email.Equals(request.Email));

        userExists.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveAnError_When_NameIsEmpty(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Name = string.Empty;

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });

        var userExists = await _dbContext.Users.AnyAsync(user =>
        user.Active &&
        user.Name.Equals(request.Name) &&
        user.Email.Equals(request.Email));

        userExists.ShouldBeFalse();
    }
}
