using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultPattern.Classes;

namespace ResultPattern.Test;

[TestClass]
public sealed class TestResult
{
    public TestContext TestContext { get; set; } = default!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    [TestMethod]
    public void Result_Success_SerializesToJson()
    {
        var result = Result<bool>.Success(true, message: "Operation completed");
        var json = JsonSerializer.Serialize(result, JsonOptions);
        TestContext.WriteLine(json);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
    }

    [TestMethod]
    public void Result_Error_WithProblems_SerializesToJson()
    {
        var problems = new[]
        {
            new ProblemDetail("Title", "Title is required"),
            new ProblemDetail("Id", "Id must be positive"),
        };
        var result = Result<string>.Failure("Validation failed", problems: problems);
        var json = JsonSerializer.Serialize(result, JsonOptions);
        TestContext.WriteLine(json);
        Assert.IsTrue(result.IsFailure);
        Assert.HasCount(2, result.Problems);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
    }

    [TestMethod]
    public void Result_Error_WithException_SerializesToJson()
    {
        var ex = new InvalidOperationException("Database connection failed");
        var result = Result<string>.Failure("Operation failed", exception: ex);
        var json = JsonSerializer.Serialize(result, JsonOptions);
        TestContext.WriteLine(json);
        Assert.IsTrue(result.IsFailure);
        Assert.IsNotNull(result.Exception);
        Assert.AreEqual("Operation failed", result.Message);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
    }

    [TestMethod]
    public void Result_Warning_SerializesToJson()
    {
        var problems = new[] { new ProblemDetail("Deprecated", "This API will be removed") };
        var result = Result<bool>.Warning("Deprecated usage", problems: problems);
        var json = JsonSerializer.Serialize(result, JsonOptions);
        TestContext.WriteLine(json);
        Assert.IsTrue(result.IsWarning);
        Assert.HasCount(1, result.Problems);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
    }
}
