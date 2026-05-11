using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Checklist.Validators;

public sealed class ChecklistItemCreateRequestValidator : IRequestValidator<ChecklistItemCreateRequest>
{
    public IReadOnlyList<string> Validate(ChecklistItemCreateRequest request)
    {
        var errors = new List<string>();
        if (request.TaskItemId == Guid.Empty) errors.Add("TaskItemId is required.");
        if (string.IsNullOrWhiteSpace(request.Text)) errors.Add("Text is required.");
        if (request.Text?.Length > 1000) errors.Add("Text must be 1000 characters or fewer.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}

public sealed class ChecklistItemUpdateRequestValidator : IRequestValidator<ChecklistItemUpdateRequest>
{
    public IReadOnlyList<string> Validate(ChecklistItemUpdateRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.Text)) errors.Add("Text is required.");
        if (request.Text?.Length > 1000) errors.Add("Text must be 1000 characters or fewer.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}
