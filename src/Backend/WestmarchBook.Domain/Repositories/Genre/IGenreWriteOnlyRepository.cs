namespace WestmarchBook.Domain.Repositories.Genre;

public interface IGenreWriteOnlyRepository
{
    Task Add(Entities.Genre genre);
    Task<bool> DeleteById(long genreId);
}
