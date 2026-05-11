namespace TaskManager.Application.Common;

public sealed record Error(string Code, string Message, IReadOnlyList<string>? Details = null)
{
    public static Error Validation(IReadOnlyList<string> details) => new("validation_error", "Validation failed", details);
    public static Error NotFound(string message) => new("not_found", message);
    public static Error Conflict(string message) => new("conflict", message);
    public static Error Unauthorized(string message = "Unauthorized") => new("unauthorized", message);
    public static Error Forbidden(string message = "Forbidden") => new("forbidden", message);
}
