namespace WestmarchBook.Domain.Repositories.Rent;

public interface IRentWriteOnlyRepository
{
    Task AddRent(Entities.Rent rent);
}
