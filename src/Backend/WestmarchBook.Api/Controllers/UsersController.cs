using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.User.Profile;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RequestRegisterUserJson request, [FromServices] IRegisterUserAccountUseCase useCase)
    {
        var result = await useCase.Execute(request);

        return Created(string.Empty, result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
    {
        var result = await useCase.Execute();

        return Ok(result);
    }
}
