namespace WestmarchBook.Domain.Messaging;

public interface IScheduleAccountDeletion
{
    Task Schedule(long userId);
}
