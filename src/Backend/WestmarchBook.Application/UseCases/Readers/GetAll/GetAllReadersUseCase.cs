using Mapster;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.Reader;

namespace WestmarchBook.Application.UseCases.Readers.GetAll;

public class GetAllReadersUseCase : IGetAllReadersUseCase
{
    private readonly IReaderReadOnlyRepository _readerReadRepository;

    public GetAllReadersUseCase(IReaderReadOnlyRepository readerReadRepository)
    {
        _readerReadRepository = readerReadRepository;
    }

    public async Task<IList<ResponseReaderJson>> Execute()
    {
        var readers = await _readerReadRepository.GetAllActive();

        return readers.Adapt<IList<ResponseReaderJson>>();
    }

    public async Task<long> GetNextReaderId()
    {
        var lastId = await _readerReadRepository.GetLastReaderId();

        return lastId + 1;
    }
}
