using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;
using WestmarchBook.Exception;

namespace WebApi.Tests.User.Update;

public class UpdateUserTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "users/profile";
    private readonly UserIdentityManager _firstUser;

    public UpdateUserTests(WestmarchBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.User_1;
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await Patch(REQUEST_URI, request, accessToken: _firstUser.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var userExists = await DbContext.Users.AnyAsync(user => user.Active && user.Id == _firstUser.GetId()
        && user.UserName.Equals(request.UserName) && user.Email.Equals(request.Email));

        userExists.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_When_UserNameIsEmpty(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        request.UserName = string.Empty;

        var response = await Patch(REQUEST_URI, request, accessToken: _firstUser.GetAccessToken(), culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_USERNAME_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString()!.Equals(expectedMessage));
        });
    }
}
