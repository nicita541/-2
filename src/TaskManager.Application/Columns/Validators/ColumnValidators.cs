using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Columns.Validators;

public sealed class ColumnCreateRequestValidator : IRequestValidator<ColumnCreateRequest>
{
    public IReadOnlyList<string> Validate(ColumnCreateRequest request)
    {
        var errors = new List<string>();
        if (request.BoardId == Guid.Empty) errors.Add("BoardId is required.");
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add("Name is required.");
        if (request.Name?.Length > 200) errors.Add("Name must be 200 characters or fewer.");
        if (request.Position < 0) errors.Add("Position must be greater than or equal to 0.");
        return errors;
    }
}
