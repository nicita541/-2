using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Comments.Validators;

public sealed class CommentCreateRequestValidator : IRequestValidator<CommentCreateRequest>
{
    public IReadOnlyList<string> Validate(CommentCreateRequest request)
    {
        var errors = new List<string>();
        if (request.TaskItemId == Guid.Empty) errors.Add("TaskItemId is required.");
        if (string.IsNullOrWhiteSpace(request.Body)) errors.Add("Body is required.");
        if (request.Body?.Length > 10000) errors.Add("Body must be 10000 characters or fewer.");
        return errors;
    }
}
