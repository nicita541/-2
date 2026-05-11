using TaskManager.Application.Common.Validation;
using TaskManager.Application.Workspaces.Dtos;

namespace TaskManager.Application.Workspaces.Validators;

public sealed class WorkspaceCreateRequestValidator : IRequestValidator<WorkspaceCreateRequest>
{
    public IReadOnlyList<string> Validate(WorkspaceCreateRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add("Name is required.");
        if (request.Name?.Length > 200) errors.Add("Name must be 200 characters or fewer.");
        if (request.Description?.Length > 2000) errors.Add("Description must be 2000 characters or fewer.");
        return errors;
    }
}

public sealed class AddWorkspaceMemberRequestValidator : IRequestValidator<AddWorkspaceMemberRequest>
{
    public IReadOnlyList<string> Validate(AddWorkspaceMemberRequest request)
    {
        var errors = new List<string>();
        if (request.UserId == Guid.Empty) errors.Add("UserId is required.");
        if (!Enum.IsDefined(request.Role)) errors.Add("Role is invalid.");
        return errors;
    }
}
