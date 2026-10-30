using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Publisher;

namespace WestmarchBook.Application.UseCases.Publisher.GetAll;

public class GetAllPublishersUseCase : IGetAllPublishersUseCase
{
    private readonly IPublisherReadOnlyRepository _publisherReadRepository;

    public GetAllPublishersUseCase(IPublisherReadOnlyRepository publisherReadRepository)
    {
        _publisherReadRepository = publisherReadRepository;
    }

    public async Task<IList<ResponsePublisherJson>> Execute()
    {
        var publishers = await _publisherReadRepository.GetAllActive();

        return publishers.Adapt<IList<ResponsePublisherJson>>();
    }
}
