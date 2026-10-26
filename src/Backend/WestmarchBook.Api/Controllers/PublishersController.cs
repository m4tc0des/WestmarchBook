using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.Publisher.Register;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class PublishersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterPublisherJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RequestRegisterPublisherJson request, [FromServices] IRegisterPublisherUseCase useCase)
    {
        var result = await useCase.Execute(request);
        return Created(string.Empty, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponsePublisherJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        return Ok();
    }
}
