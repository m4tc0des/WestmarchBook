using Microsoft.AspNetCore.Mvc;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Communication.Requests;

namespace WestmarchBook.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Register([FromBody] RequestRegisterUserJson request, [FromServices] IRegisterUserAccountUseCase useCase)
    {
        useCase.Execute(request);

        return Created();
    }
}
