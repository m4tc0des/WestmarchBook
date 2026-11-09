 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Application.UseCases.PasswordRecovery.RequestCode;
using WestmarchBook.Application.UseCases.PasswordRecovery.ResetPassword;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] RequestLoginJson request, [FromServices] ILoginWithEmailAndPasswordUseCase useCase)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }

    [HttpPost("password-recovery")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PasswordRecovery([FromBody] RequestPasswordRecoveryCodeJson request, [FromServices] IRequestPasswordRecoveryCodeUseCase useCase)
    {
        await useCase.Execute(request);

        return Accepted();
    }

    [HttpPost("password-recovery/reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword([FromBody] RequestResetPasswordJson request, [FromServices] IResetPasswordUseCase useCase)
    {
        await useCase.Execute(request);
        return NoContent();
    }
}
