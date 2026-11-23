using FluentValidation;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Publisher.Update;

public class UpdatePublisherValidator : AbstractValidator<RequestUpdatePublisherJson>
{
    public UpdatePublisherValidator()
    {
        RuleFor(publisher => publisher.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_REQUIRED)
            .MaximumLength(100)
            .WithMessage(ResourceMessagesException.VALIDATION_PUBLISHER_NAME_MAX_LENGTH);
    }
}
