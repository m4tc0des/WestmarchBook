using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Publisher.Update;

public class UpdatePublisherUseCase : IUpdatePublisherUseCase
{
    private readonly IPublisherUpdateOnlyRepository _publisherUpdateRepository;
    private readonly IPublisherReadOnlyRepository _publisherReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePublisherUseCase(
        IPublisherUpdateOnlyRepository publisherUpdateRepository,
        IPublisherReadOnlyRepository publisherReadRepository,
        IUnitOfWork unitOfWork)
    {
        _publisherUpdateRepository = publisherUpdateRepository;
        _publisherReadRepository = publisherReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long id, RequestUpdatePublisherJson request)
    {
        var publisher = await _publisherUpdateRepository.GetById(id);

        if (publisher is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }

        await ValidateAndThrowOnFailures(id, request);

        publisher.Name = request.Name.Trim();

        _publisherUpdateRepository.Update(publisher);

        await _unitOfWork.Commit();
    }

    private async Task ValidateAndThrowOnFailures(long currentPublisherId, RequestUpdatePublisherJson request)
    {
        var result = new UpdatePublisherValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var existsPublisherWithSameName = await _publisherReadRepository.ExistActivePublisherWithName(request.Name.Trim(), currentPublisherId);

        if (existsPublisherWithSameName)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_PUBLISHER_NAME_ALREADY_EXISTS]);
        }
    }
}
