using CommonTestUtilities.Requests;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WebApi.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordTests: BaseIntegrationTest
{
    private const string REQUEST_URI = "/authentication";
    private readonly UserIdentityManager _user1;

    public LoginWithEmailAndPasswordTests(WestmarchBookApplicationFactory factory): base(factory)
    {
        _user1 = factory.User_1;
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson
        {
            Email = _user1.GetEmail(),
            Password = _user1.GetPassword(),
        };

        var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(_user1.GetName());
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldThrowException_When_UserDontExist(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_LOGIN_INVALID", new CultureInfo(culture));

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}
