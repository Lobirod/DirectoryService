namespace Shared;

public record Error
{
    private const string SEPARATOR = "||";
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

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        string[] parts = serialized.Split(SEPARATOR);

        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        return new Error(parts[0], parts[1], type);
    }

    public Errors ToErrors() => this;
}