namespace WestmarchBook.Domain.Messaging;

public sealed record AccountDeletionMessage(long UserId, DateTime ScheduledAt);
