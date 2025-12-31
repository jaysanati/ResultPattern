using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResultPattern.Classes;

public record ProblemDetail
{
    public string Property { get; }
    public string Message { get; }
    public ProblemDetail(string property, string message)
    {
        Property = property;
        Message = message;
    }
}

public enum ResultKind
{
    Success,
    Error,
    Warning
}

public record Result<T>
{
    [JsonPropertyName("Status")]
    public ResultKind Kind { get; init; }

    [JsonIgnore]
    public bool IsSuccess => Kind == ResultKind.Success;

    [JsonIgnore]
    public bool IsFailure => Kind == ResultKind.Error;

    [JsonIgnore]
    public bool IsWarning => Kind == ResultKind.Warning;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Value { get; init; }

    public IReadOnlyList<ProblemDetail> Problems { get; init; }
    
    [JsonIgnore]
    public Exception? Exception { get; }

    public Result()
    {
    }

    private Result(ResultKind kind, string? message = default, T? value = default, IEnumerable<ProblemDetail>? problems = null, Exception? exception = null)
    {
        if (kind == ResultKind.Error && string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Error must have a non-empty message.", nameof(message));
        }

        Kind = kind;
        Message = message;
        Value = value;
        Exception = exception;

        if (problems is IReadOnlyList<ProblemDetail> r)
        {
            Problems = r;
        }
        else if (problems is null)
        {
            Problems = [];
        }
        else
        {
            Problems = [.. problems];
        }
    }

    public static Result<T> Success(T value, string? message = null)
        => new(ResultKind.Success, message, value, null, null);

    public static Result<T> Failure(string message, IEnumerable<ProblemDetail>? problems = null, Exception? exception = null, T? value = default)
        => new(ResultKind.Error, message, value, problems, exception);

    public static Result<T> Warning(string message, IEnumerable<ProblemDetail>? problems = null, T? value = default)
        => new(ResultKind.Warning, message, value, problems, null);

    public override string ToString()
        => Kind switch
        {
            ResultKind.Success => string.IsNullOrWhiteSpace(Message) ? $"Success({Value})" : $"Success({Message})",
            ResultKind.Error => Problems.Count > 0 ? $"Error(Problems: {Problems.Count})" : $"Error({Message})",
            ResultKind.Warning => string.IsNullOrWhiteSpace(Message) ? "Warning" : $"Warning({Message})",
            _ => Message ?? string.Empty
        };
}