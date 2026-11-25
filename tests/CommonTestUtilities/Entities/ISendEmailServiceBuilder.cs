using Moq;
using WestmarchBook.Domain.Services.EmailService;

namespace CommonTestUtilities.Entities;

public class ISendEmailServiceBuilder
{
    public static ISendEmailService Build()
    {
        var mock = new Mock<ISendEmailService>();
        
        return mock.Object;
    }
}
