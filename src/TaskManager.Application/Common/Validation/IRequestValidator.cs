namespace TaskManager.Application.Common.Validation;

public interface IRequestValidator
{
    Type RequestType { get; }
    IReadOnlyList<string> ValidateObject(object request);
}

public interface IRequestValidator<in TRequest> : IRequestValidator
{
    IReadOnlyList<string> Validate(TRequest request);

    Type IRequestValidator.RequestType => typeof(TRequest);

    IReadOnlyList<string> IRequestValidator.ValidateObject(object request) =>
        request is TRequest typed ? Validate(typed) : [];
}
