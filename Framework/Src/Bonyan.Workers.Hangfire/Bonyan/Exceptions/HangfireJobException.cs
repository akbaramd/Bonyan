using Microsoft.Extensions.Logging;

namespace Bonyan.Exceptions;

[Serializable]
public class HangfireJobException(
    string? message = "An error occurred while managing a Hangfire job.",
    string? code = "HANGFIRE_JOB_ERROR",
    string? details = null,
    Type? jobType = null,
    string? operation = null,
    Exception? innerException = null,
    LogLevel logLevel = LogLevel.Error)
    : BusinessException(code, message, details, innerException, logLevel)
{
    /// <summary>
    /// Gets the type of the Hangfire job associated with the exception.
    /// </summary>
    public Type? JobType { get; } = jobType;

    /// <summary>
    /// Gets the specific Hangfire operation (e.g., Enqueue, ScheduleRecurring, Cancel) associated with the exception.
    /// </summary>
    public string? Operation { get; } = operation;

    /// <summary>
    /// Adds custom data to the exception's data dictionary and returns the modified exception.
    /// </summary>
    /// <param name="name">The key for the custom data.</param>
    /// <param name="value">The value for the custom data.</param>
    /// <returns>The current exception instance with the added data.</returns>
    public HangfireJobException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }

    /// <summary>
    /// Provides a string representation of the Hangfire job exception, including job type and operation details.
    /// </summary>
    /// <returns>A string representation of the exception.</returns>
    public override string ToString()
    {
        var baseMessage = base.ToString();
        var jobTypeMessage = JobType != null ? $"JobType: {JobType.Name}, " : "";
        var operationMessage = Operation != null ? $"Operation: {Operation}, " : "";
        return $"{baseMessage}, {jobTypeMessage}{operationMessage}LogLevel: {LogLevel}";
    }
}