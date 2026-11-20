using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Publisher.GetById;

public class GetPublisherByIdUseCase : IGetPublisherByIdUseCase
{
    private readonly IPublisherReadOnlyRepository _publisherReadRepository;

    public GetPublisherByIdUseCase(IPublisherReadOnlyRepository publisherReadRepository)
    {
        _publisherReadRepository = publisherReadRepository;
    }

    public async Task<ResponsePublisherJson> Execute(long id)
    {
        var publisher = await _publisherReadRepository.GetById(id);

        if (publisher is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }

        return publisher.Adapt<ResponsePublisherJson>();
    }
}
