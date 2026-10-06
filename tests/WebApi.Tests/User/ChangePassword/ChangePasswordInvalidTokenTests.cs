using Shouldly;
using System.Net;
using WestmarchBook.Communication.Requests;

namespace WebApi.Tests.User.ChangePassword;

public class ChangePasswordInvalidTokenTests: BaseIntegrationTest
{
    private const string REQUEST_URI = "users/password";
    private readonly string _tokenUserNotExistDatabase;

    public ChangePasswordInvalidTokenTests(WestmarchBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = new RequestChangePasswordJson();

        var response = await Patch(REQUEST_URI, request, accessToken: "invalid-token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson();

        var response = await Patch(REQUEST_URI, request, accessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var request = new RequestChangePasswordJson();

        var response = await Patch(REQUEST_URI, request, accessToken: _tokenUserNotExistDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
