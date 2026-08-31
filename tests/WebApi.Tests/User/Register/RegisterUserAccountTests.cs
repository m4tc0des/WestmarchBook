using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WestmarchBook.Exception;

namespace WebApi.Tests.User.Register;

public class RegisterUserAccountTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/users";

    public RegisterUserAccountTests(WestmarchBookApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeEmpty();

        var userExists = await DbContext.Users.AnyAsync(user =>
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

        var response = await Post(REQUEST_URI, request, culture);

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

        var userExists = await DbContext.Users.AnyAsync(user =>
        user.Active &&
        user.Name.Equals(request.Name) &&
        user.Email.Equals(request.Email));

        userExists.ShouldBeFalse();
    }
}
