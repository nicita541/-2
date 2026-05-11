using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Attachment : Entity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public Guid UploadedById { get; set; }
    public ApplicationUser? UploadedBy { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required string Url { get; set; }
    public long SizeBytes { get; set; }
}
