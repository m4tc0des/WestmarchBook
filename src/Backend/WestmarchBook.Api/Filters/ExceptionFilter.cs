using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is WestmarchBookException westmarchBookException)
        {
            context.HttpContext.Response.StatusCode = (int)westmarchBookException.GetStatusCode();
            context.Result = new ObjectResult(new ResponseErrorJson(westmarchBookException.GetErrorMessage()));
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
        }
    }
}
