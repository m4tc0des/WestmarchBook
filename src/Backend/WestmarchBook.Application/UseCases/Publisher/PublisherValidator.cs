using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Publisher;

public class PublisherValidator : AbstractValidator<RequestRegisterPublisherJson>
{
    public PublisherValidator()
    {
        RuleFor(publisher => publisher.Name)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_REQUIRED)
            .MaximumLength(100)
            .WithMessage(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_MAX_LENGTH);
    }
}
