using TaskManager.Application.Common.Validation;
using TaskManager.Application.TaskItems.Dtos;

namespace TaskManager.Application.TaskItems.Validators;

public sealed class TaskItemCreateRequestValidator : IRequestValidator<TaskItemCreateRequest>
{
    public IReadOnlyList<string> Validate(TaskItemCreateRequest request)
    {
        var errors = new List<string>();
        if (request.BoardColumnId == Guid.Empty) errors.Add("BoardColumnId is required.");
        if (string.IsNullOrWhiteSpace(request.Title)) errors.Add("Title is required.");
        if (request.Title?.Length > 500) errors.Add("Title must be 500 characters or fewer.");
        if (request.Description?.Length > 10000) errors.Add("Description must be 10000 characters or fewer.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}

public sealed class TaskItemUpdateRequestValidator : IRequestValidator<TaskItemUpdateRequest>
{
    public IReadOnlyList<string> Validate(TaskItemUpdateRequest request)
    {
        var errors = new List<string>();
        if (request.BoardColumnId == Guid.Empty) errors.Add("BoardColumnId is required.");
        if (string.IsNullOrWhiteSpace(request.Title)) errors.Add("Title is required.");
        if (request.Title?.Length > 500) errors.Add("Title must be 500 characters or fewer.");
        if (request.Description?.Length > 10000) errors.Add("Description must be 10000 characters or fewer.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}

public sealed class TaskItemMoveRequestValidator : IRequestValidator<TaskItemMoveRequest>
{
    public IReadOnlyList<string> Validate(TaskItemMoveRequest request)
    {
        var errors = new List<string>();
        if (request.BoardColumnId == Guid.Empty) errors.Add("BoardColumnId is required.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}
