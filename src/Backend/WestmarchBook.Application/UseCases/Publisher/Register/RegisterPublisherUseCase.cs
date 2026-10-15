using Mapster;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Publisher;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Publisher.Register;

public class RegisterPublisherUseCase : IRegisterPublisherUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IPublisherWriteOnlyRepository _publisherRepository;
    private readonly IPublisherReadOnlyRepository _publisherReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterPublisherUseCase(ILoggedUser loggedUser, IPublisherWriteOnlyRepository publisherRepository, IPublisherReadOnlyRepository publisherReadRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _publisherRepository = publisherRepository;
        _publisherReadRepository = publisherReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterPublisherJson> Execute(RequestRegisterPublisherJson request)
    {
        await Validate(request);

        var publisher = request.Adapt<Domain.Entities.Publisher>();

        await _publisherRepository.Add(publisher);
        await _unitOfWork.Commit();

        return new ResponseRegisterPublisherJson
        {
            Id = publisher.Id,
            Name = publisher.Name
        };
    }

    private async Task Validate(RequestRegisterPublisherJson request)
    {
        var result = new PublisherValidator().Validate(request);
        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var publisherExists = await _publisherReadRepository.ExistActivePublisherWithName(request.Name);

        if (publisherExists)
        {
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.VALIDATION_PUBLISHER_NAME_ALREADY_EXISTS });
        }
    }
}
