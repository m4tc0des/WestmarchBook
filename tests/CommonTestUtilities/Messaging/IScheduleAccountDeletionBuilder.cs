using Moq;
using WestmarchBook.Domain.Messaging;

namespace CommonTestUtilities.Messaging;

public class IScheduleAccountDeletionBuilder
{
    private readonly Mock<IScheduleAccountDeletion> _mock = new();

    public IScheduleAccountDeletion BuildInstance() => _mock.Object;

    public void VerifyScheduleCalled(long userId, int times = 1)
    {
        _mock.Verify(scheduler => scheduler.Schedule(userId), Times.Exactly(times));
    }

    public static IScheduleAccountDeletion Build()
    {
        return new Mock<IScheduleAccountDeletion>().Object;
    }
}
