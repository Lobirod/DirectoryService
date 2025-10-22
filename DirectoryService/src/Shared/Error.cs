namespace Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error NotFound(string? code, string message, string? invalidField = null) =>
        new Error(code ?? "record.not.found", message, ErrorType.NOT_FOUND, invalidField);

    public static Error Validation(string? code, string message, string? invalidField = null) =>
        new Error(code ?? "record.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error Conflict(string? code, string message, string? invalidField = null) =>
        new Error(code ?? "record.is.conflict", message, ErrorType.CONFLICT, invalidField);

    public static Error Failure(string? code, string message, string? invalidField = null) =>
        new Error(code ?? "failure", message, ErrorType.FAILURE, invalidField);

    public Errors ToErrors() => this;
}