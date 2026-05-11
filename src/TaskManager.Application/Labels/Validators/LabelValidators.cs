using System.Text.RegularExpressions;
using TaskManager.Application.Common.Validation;
using TaskManager.Application.Labels.Dtos;

namespace TaskManager.Application.Labels.Validators;

public sealed partial class LabelCreateRequestValidator : IRequestValidator<LabelCreateRequest>
{
    public IReadOnlyList<string> Validate(LabelCreateRequest request)
    {
        var errors = new List<string>();
        if (request.BoardId == Guid.Empty) errors.Add("BoardId is required.");
        if (string.IsNullOrWhiteSpace(request.Name)) errors.Add("Name is required.");
        if (request.Name?.Length > 100) errors.Add("Name must be 100 characters or fewer.");
        if (string.IsNullOrWhiteSpace(request.ColorHex) || !ColorRegex().IsMatch(request.ColorHex)) errors.Add("ColorHex must be a valid #RRGGBB value.");
        return errors;
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex ColorRegex();
}
