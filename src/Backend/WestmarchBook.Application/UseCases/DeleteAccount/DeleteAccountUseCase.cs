using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Messaging;
using WestmarchBook.Domain.Repositories.User;

namespace WestmarchBook.Application.UseCases.DeleteAccount;

public class DeleteAccountUseCase : IDeleteAccountUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IScheduleAccountDeletion _scheduleAccountDeletion;

    public DeleteAccountUseCase(ILoggedUser loggedUser,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IScheduleAccountDeletion scheduleAccountDeletion)
    {
        _loggedUser = loggedUser;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _scheduleAccountDeletion = scheduleAccountDeletion;
    }

    public async Task Execute()
    {
        var loggedUserId = _loggedUser.GetUserId();

        await _userWriteOnlyRepository.DeactivateAccount(loggedUserId);
        await _scheduleAccountDeletion.Schedule(loggedUserId);
    }
}
