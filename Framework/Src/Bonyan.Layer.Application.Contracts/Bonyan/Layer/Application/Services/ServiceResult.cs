namespace Bonyan.Layer.Application.Abstractions;

/// <summary>
/// Represents the result of a service operation.
/// </summary>
[Serializable]
public class ServiceResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    private static readonly ServiceResult SuccessResult = new ServiceResult(true);

    protected ServiceResult(bool isSuccess, string errorMessage = null, string errorCode = null, List<string> validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        ValidationErrors = validationErrors ?? new List<string>();
    }

    public static ServiceResult Success() => SuccessResult;

    public static ServiceResult Failure(string errorMessage, string errorCode = null, List<string> validationErrors = null) =>
        new ServiceResult(false, errorMessage, errorCode, validationErrors);

    public void EnsureSuccess()
    {
        if (IsFailure)
            throw new InvalidOperationException(ErrorMessage ?? "Operation failed without a specific error message.");
    }
}

/// <summary>
/// Represents the result of an operation with success or failure state.
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
    public T Value { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    private ServiceResult(T value, bool isSuccess, string errorMessage = null, string errorCode = null, List<string> validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Value = value;
        ValidationErrors = validationErrors ?? new List<string>();
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static ServiceResult<T> Success(T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value), "Value cannot be null in a success result.");
        return new ServiceResult<T>(value, true);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static ServiceResult<T> Failure(string errorMessage, string errorCode = null, List<string> validationErrors = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));

        return new ServiceResult<T>(default, false, errorMessage, errorCode, validationErrors);
    }

    /// <summary>
    /// Ensures the operation was successful; throws if it was not.
    /// </summary>
    public void EnsureSuccess()
    {
        if (IsFailure)
            throw new InvalidOperationException(ErrorMessage ?? "Operation failed.");
    }

    /// <summary>
    /// Maps the current result to a new result of a different type.
    /// </summary>
    public ServiceResult<U> Map<U>(Func<T, U> mapper)
    {
        if (mapper == null) throw new ArgumentNullException(nameof(mapper));

        return IsSuccess
            ? ServiceResult<U>.Success(mapper(Value))
            : ServiceResult<U>.Failure(ErrorMessage, ErrorCode, ValidationErrors as List<string>);
    }

    /// <summary>
    /// Chains the current result to another operation that returns a result.
    /// </summary>
    public ServiceResult<U> Bind<U>(Func<T, ServiceResult<U>> binder)
    {
        if (binder == null) throw new ArgumentNullException(nameof(binder));

        return IsSuccess ? binder(Value) : ServiceResult<U>.Failure(ErrorMessage, ErrorCode, ValidationErrors as List<string>);
    }
}
