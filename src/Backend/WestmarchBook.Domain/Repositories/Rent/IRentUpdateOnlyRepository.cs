namespace WestmarchBook.Domain.Repositories.Rent;

public interface IRentUpdateOnlyRepository
{
    Task<Entities.Rent?> GetById(long rentId);
    void Update(Entities.Rent rent);
    Task<bool> DeleteById(long rentId);
}
