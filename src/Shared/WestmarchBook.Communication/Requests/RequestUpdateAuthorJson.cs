using Microsoft.AspNetCore.Http;

namespace WestmarchBook.Communication.Requests;

public class RequestUpdateAuthorJson
{
    public string? Name { get; set; }
    public IFormFile? AuthorIllustration { get; set; }
}
