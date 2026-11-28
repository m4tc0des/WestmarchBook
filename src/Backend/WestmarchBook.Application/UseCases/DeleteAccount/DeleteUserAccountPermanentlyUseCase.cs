using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Services.StorageService;

namespace WestmarchBook.Application.UseCases.DeleteAccount;

public class DeleteUserAccountPermanentlyUseCase : IDeleteUserAccountPermanentlyUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserAccountPermanentlyUseCase(
        IUserReadOnlyRepository userReadOnlyRepository,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _userReadOnlyRepository = userReadOnlyRepository;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long userId)
    {
        var user = await _userReadOnlyRepository.GetByIdForDeletion(userId);

        if (user is null)
        {
            return;
        }

        if (user.HasImage)
        {
            await _storageService.DeleteUserFiles(user);
        }

        await _userWriteOnlyRepository.DeleteAccount(userId);

        await _unitOfWork.Commit();
    }
}
