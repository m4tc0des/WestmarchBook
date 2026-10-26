using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.Book.GetById;
using WestmarchBook.Application.UseCases.Book.Register;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterBookJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RequestRegisterBookJson request, [FromServices] IRegisterBookUseCase useCase)
    {
        var result = await useCase.Execute(request);

        return Created(string.Empty, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseBookJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, [FromServices] IGetBookByIdUseCase useCase)
    {
        var book = await useCase.Execute(id);
        return Ok(book);
    }
}
