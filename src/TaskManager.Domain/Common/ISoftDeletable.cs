namespace TaskManager.Domain.Common;

public interface ISoftDeletable
{
    DateTime? DeletedAtUtc { get; set; }
}
