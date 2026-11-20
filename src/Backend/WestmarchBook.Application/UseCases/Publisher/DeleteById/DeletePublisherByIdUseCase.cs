using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Publisher.DeleteById;

public class DeletePublisherByIdUseCase : IDeletePublisherByIdUseCase
{
    private readonly IPublisherReadOnlyRepository _publisherReadRepository;
    private readonly IPublisherWriteOnlyRepository _publisherWriteRepository;

    public DeletePublisherByIdUseCase(
        IPublisherReadOnlyRepository publisherReadRepository,
        IPublisherWriteOnlyRepository publisherWriteRepository)
    {
        _publisherReadRepository = publisherReadRepository;
        _publisherWriteRepository = publisherWriteRepository;
    }

    public async Task Execute(long id)
    {
        var publisher = await _publisherReadRepository.GetById(id);

        if (publisher is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }

        if (await _publisherReadRepository.HasBooksLinked(id))
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_PUBLISHER_HAS_LINKED_BOOKS]);
        }

        var deleted = await _publisherWriteRepository.DeleteById(id);

        if (deleted == false)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_PUBLISHER_NOT_FOUND);
        }
    }
}
