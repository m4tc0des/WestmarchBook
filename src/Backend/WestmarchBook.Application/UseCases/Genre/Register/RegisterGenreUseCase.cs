using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.Genre;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Genre.Register;

public class RegisterGenreUseCase : IRegisterGenreUseCase
{
    private readonly IGenreWriteOnlyRepository _genreRepository;
    private readonly IGenreReadOnlyRepository _genreReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterGenreUseCase(
        IGenreWriteOnlyRepository genreRepository,
        IGenreReadOnlyRepository genreReadRepository,
        IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
        _genreReadRepository = genreReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisterGenreJson> Execute(RequestRegisterGenreJson request)
    {
        await Validate(request);

        var genre = new Domain.Entities.Genre
        {
            Name = request.Name.Trim()
        };

        await _genreRepository.Add(genre);
        await _unitOfWork.Commit();

        return new ResponseRegisterGenreJson
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }

    private async Task Validate(RequestRegisterGenreJson request)
    {
        var result = new GenreValidator().Validate(request);
        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var genreExists = await _genreReadRepository.ExistActiveGenreWithName(request.Name.Trim());
        if (genreExists)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_GENRE_NAME_ALREADY_EXISTS]);
        }
    }
}
