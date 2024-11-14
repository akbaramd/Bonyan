namespace Bonyan.Layer.Domain.DomainService;

public class BonDomainResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }

    protected BonDomainResult(bool isSuccess, string errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static BonDomainResult Success() => new BonDomainResult(true);

    public static BonDomainResult Failure(string errorMessage) => new BonDomainResult(false, errorMessage);

    public void EnsureSuccess()
    {
        if (IsFailure)
            throw new InvalidOperationException(ErrorMessage ?? "Operation failed without a specific error message.");
    }
}

public class BonDomainResult<T> : BonDomainResult
{
    public T Value { get; }

    private BonDomainResult(T value, bool isSuccess, string errorMessage = null)
        : base(isSuccess, errorMessage)
    {
        Value = value;
    }

    public static BonDomainResult<T> Success(T value) => new BonDomainResult<T>(value, true);

    public static new BonDomainResult<T> Failure(string errorMessage) => new BonDomainResult<T>(default, false, errorMessage);
}