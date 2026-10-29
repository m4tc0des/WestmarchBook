using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.Readers.Register;
using WestmarchBook.Application.UseCases.Readers.Update;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class ReadersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterReaderJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RequestRegisterReaderJson request, [FromServices] IRegisterReaderAccountUseCase useCase)
    {
        var result = await useCase.Execute(request);

        return Created(string.Empty, result);
    }

    [HttpPatch("profile/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromRoute] long id, [FromBody] RequestUpdateReaderJson request, [FromServices] IUpdateReaderUseCase useCase)
    {
        await useCase.Execute(id, request);

        return NoContent();
    }
}
