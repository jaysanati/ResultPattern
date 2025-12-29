using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResultPattern.Classes;

public readonly struct ProblemDetail
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

[JsonConverter(typeof(ResultJsonConverterFactory))]
public readonly struct Result<T>
{
    public ResultKind Kind { get; }
    public bool IsSuccess => Kind == ResultKind.Success;
    public bool IsFailure => Kind == ResultKind.Error;
    public bool IsWarning => Kind == ResultKind.Warning;

    public T? Value { get; }
    public string? Message { get; }
    public IReadOnlyList<ProblemDetail> Problems { get; }
    public Exception? Exception { get; }

    private Result(ResultKind kind, string? message = null, T? value = default, IEnumerable<ProblemDetail>? problems = null, Exception? exception = null)
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

    public static Result<T> Warning(string message, T? value = default, IEnumerable<ProblemDetail>? problems = null)
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

public class ResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var tArg = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ResultJsonConverter<>).MakeGenericType(tArg);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization of Result<T> is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("kind", value.Kind.ToString());

        if (value.Message is not null)
        {
            writer.WriteString("message", value.Message);
        }

        if (value.Value is not null)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }

        switch (value.Kind)
        {
            case ResultKind.Success:
                // Do not include Problems on success
                break;
            case ResultKind.Error:
                if (value.Exception is not null)
                {
                    writer.WriteString("exception", value.Exception.Message);
                }
                if (value.Problems.Count > 0)
                {
                    writer.WritePropertyName("problems");
                    JsonSerializer.Serialize(writer, value.Problems, options);
                }
                break;
            case ResultKind.Warning:
                if (value.Problems.Count > 0)
                {
                    writer.WritePropertyName("problems");
                    JsonSerializer.Serialize(writer, value.Problems, options);
                }
                break;
        }

        writer.WriteEndObject();
    }
}