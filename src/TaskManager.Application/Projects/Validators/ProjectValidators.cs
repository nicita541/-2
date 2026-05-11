using TaskManager.Application.Common.Validation;
using TaskManager.Application.Projects.Dtos;

namespace TaskManager.Application.Projects.Validators;

public sealed class ProjectCreateRequestValidator : IRequestValidator<ProjectCreateRequest>
{
    public IReadOnlyList<string> Validate(ProjectCreateRequest request)
    {
        var errors = new List<string>();
        if (request.WorkspaceId == Guid.Empty) errors.Add("WorkspaceId is required.");
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add("Name is required.");
        if (request.Name?.Length > 200) errors.Add("Name must be 200 characters or fewer.");
        if (request.Description?.Length > 2000) errors.Add("Description must be 2000 characters or fewer.");
        return errors;
    }
}
