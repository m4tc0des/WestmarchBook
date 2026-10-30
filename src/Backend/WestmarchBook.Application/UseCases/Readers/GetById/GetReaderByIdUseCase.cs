using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Reader;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Readers.GetById;

public class GetReaderByIdUseCase : IGetReaderByIdUseCase
{
    private readonly IReaderReadOnlyRepository _readerReadRepository;

    public GetReaderByIdUseCase(IReaderReadOnlyRepository readerReadRepository)
    {
        _readerReadRepository = readerReadRepository;
    }

    public async Task<ResponseReaderJson> Execute(long id)
    {
        var reader = await _readerReadRepository.GetById(id);

        if (reader is null)
        {
            throw new NotFoundException(ResourceMessagesException.VALIDATION_READER_NOT_FOUND);
        }

        return reader.Adapt<ResponseReaderJson>();
    }
}
