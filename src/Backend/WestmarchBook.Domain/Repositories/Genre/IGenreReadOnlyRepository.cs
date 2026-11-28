namespace WestmarchBook.Domain.Repositories.Genre;

public interface IGenreReadOnlyRepository
{
    Task<bool> ExistActiveGenreWithName(string name);
    Task<bool> ExistActiveGenreWithName(string name, long excludeGenreId);
    Task<bool> ExistActiveGenresWithIds(IList<long> genreIds);
    Task<bool> HasBooksLinked(long genreId);
    Task<Entities.Genre?> GetById(long genreId);
    Task<IList<Entities.Genre>> GetAllActive();
}
