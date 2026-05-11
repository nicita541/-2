using TaskManager.Application.Auth.Dtos;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Application.Auth.Validators;

public sealed class RegisterRequestValidator : IRequestValidator<RegisterRequest>
{
    public IReadOnlyList<string> Validate(RegisterRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@')) errors.Add("Email must be valid.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8) errors.Add("Password must contain at least 8 characters.");
        if (request.DisplayName?.Length > 200) errors.Add("DisplayName must be 200 characters or fewer.");
        return errors;
    }
}

public sealed class LoginRequestValidator : IRequestValidator<LoginRequest>
{
    public IReadOnlyList<string> Validate(LoginRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@')) errors.Add("Email must be valid.");
        if (string.IsNullOrWhiteSpace(request.Password)) errors.Add("Password is required.");
        return errors;
    }
}
