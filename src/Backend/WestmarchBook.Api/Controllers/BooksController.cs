using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.Book.ActiveById;
using WestmarchBook.Application.UseCases.Book.DeleteById;
using WestmarchBook.Application.UseCases.Book.GetById;
using WestmarchBook.Application.UseCases.Book.InactiveById;
using WestmarchBook.Application.UseCases.Book.Register;
using WestmarchBook.Application.UseCases.Book.Update;
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateById([FromRoute] long id, [FromBody] RequestUpdateBookJson request, [FromServices] IUpdateBookUseCase useCase)
    {
        await useCase.Execute(id, request);

        return NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseBookJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, [FromServices] IGetBookByIdUseCase useCase)
    {
        var book = await useCase.Execute(id);
        return Ok(book);
    }

    [HttpPatch("active/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActiveById([FromRoute] long id, [FromServices] IActiveBookByIdUseCase useCase)
    {
        await useCase.Execute(id);

        return NoContent();
    }

    [HttpPatch("inactive/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InactiveById([FromRoute] long id, [FromServices] IInactiveBookByIdUseCase useCase)
    {
        await useCase.Execute(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] long id, [FromServices] IDeleteBookByIdUseCase useCase)
    {
        await useCase.Execute(id);

        return NoContent();
    }
}
