namespace TaskManager.Application.Common;

public sealed record Result<T>(bool Succeeded, T? Value, Error? Error)
{
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
}
