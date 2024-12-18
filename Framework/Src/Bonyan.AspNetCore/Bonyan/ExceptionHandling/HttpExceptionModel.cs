namespace Bonyan.ExceptionHandling;

public record HttpExceptionModel(string Type,string Code, string Message, int Status, object? Parameters = null,string? StackTrace = null);
