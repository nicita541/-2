using TaskManager.Application.Attachments.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Attachments.Validators;

public sealed class AttachmentCreateRequestValidator : IRequestValidator<AttachmentCreateRequest>
{
    public IReadOnlyList<string> Validate(AttachmentCreateRequest request)
    {
        var errors = new List<string>();
        if (request.TaskItemId == Guid.Empty) errors.Add("TaskItemId is required.");
        if (string.IsNullOrWhiteSpace(request.FileName)) errors.Add("FileName is required.");
        if (request.FileName?.Length > 500) errors.Add("FileName must be 500 characters or fewer.");
        if (string.IsNullOrWhiteSpace(request.ContentType)) errors.Add("ContentType is required.");
        if (request.ContentType?.Length > 200) errors.Add("ContentType must be 200 characters or fewer.");
        if (string.IsNullOrWhiteSpace(request.Url)) errors.Add("Url is required.");
        if (request.Url?.Length > 2000) errors.Add("Url must be 2000 characters or fewer.");
        if (request.SizeBytes < 0) errors.Add("SizeBytes must be greater than or equal to 0.");
        return errors;
    }
}
