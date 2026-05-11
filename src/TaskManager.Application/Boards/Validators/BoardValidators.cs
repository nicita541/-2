using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Boards.Validators;

public sealed class BoardCreateRequestValidator : IRequestValidator<BoardCreateRequest>
{
    public IReadOnlyList<string> Validate(BoardCreateRequest request)
    {
        var errors = new List<string>();
        if (request.ProjectId == Guid.Empty) errors.Add("ProjectId is required.");
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add("Name is required.");
        if (request.Name?.Length > 200) errors.Add("Name must be 200 characters or fewer.");
        return errors;
    }
}
