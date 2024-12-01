using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Bonyan.Layer.Application.Services
{
    [Serializable]
    public class ServiceResult : ISerializable
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string ErrorMessage { get; private set; }
        public string ErrorCode { get; private set; }
        public IReadOnlyList<string> ValidationErrors { get; private set; }
        public Dictionary<string, string> ValidationDetails { get; private set; } = new();
        public List<ServiceResult> InnerResults { get; } = new();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string CorrelationId { get; private set; }

        protected ServiceResult(bool isSuccess, string errorMessage = null, string errorCode = null, List<string> validationErrors = null, Dictionary<string, string> validationDetails = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors ?? new List<string>();
            if (validationDetails != null)
                ValidationDetails = validationDetails;
        }

        // Factory methods for success and failure
        public static ServiceResult Success() => new ServiceResult(true);

        public static ServiceResult Failure(string errorMessage, string errorCode = null, List<string> validationErrors = null, Dictionary<string, string> validationDetails = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));

            return new ServiceResult(false, errorMessage, errorCode, validationErrors, validationDetails);
        }

        // Fluent methods
        public ServiceResult WithCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
            return this;
        }

        public ServiceResult AddValidationError(string error)
        {
            var errors = ValidationErrors.ToList();
            errors.Add(error);
            ValidationErrors = errors;
            return this;
        }

        public ServiceResult AddValidationDetail(string fieldName, string errorDetail)
        {
            ValidationDetails[fieldName] = errorDetail;
            return this;
        }

        public ServiceResult AddInnerResult(ServiceResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            InnerResults.Add(result);
            return this;
        }

        public static ServiceResult Aggregate(IEnumerable<ServiceResult> results)
        {
            var resultList = results.ToList();
            if (resultList.All(r => r.IsSuccess))
                return Success();

            var allErrors = resultList.SelectMany(r => r.ValidationErrors).ToList();
            var allDetails = resultList.SelectMany(r => r.ValidationDetails).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return Failure("One or more operations failed.", validationErrors: allErrors, validationDetails: allDetails);
        }

        public override string ToString()
        {
            return IsSuccess
                ? "Success"
                : $"Failure: {ErrorMessage}, ErrorCode: {ErrorCode}, ValidationErrors: {string.Join(", ", ValidationErrors)}, ValidationDetails: {string.Join(", ", ValidationDetails.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
        }

        public virtual void LogResult(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            if (IsSuccess)
                logger.LogInformation($"Service operation succeeded. Timestamp: {Timestamp}");
            else
                logger.LogError($"Service operation failed. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}, Timestamp: {Timestamp}, ValidationDetails: {string.Join(", ", ValidationDetails)}");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(IsSuccess), IsSuccess);
            info.AddValue(nameof(ErrorMessage), ErrorMessage);
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(ValidationErrors), ValidationErrors);
            info.AddValue(nameof(ValidationDetails), ValidationDetails);
            info.AddValue(nameof(Timestamp), Timestamp);
            info.AddValue(nameof(CorrelationId), CorrelationId);
        }
    }

    [Serializable]
    public class ServiceResult<T> : ServiceResult
    {
        public T Value { get; }

        private ServiceResult(T value, bool isSuccess, string errorMessage = null, string errorCode = null, List<string> validationErrors = null, Dictionary<string, string> validationDetails = null)
            : base(isSuccess, errorMessage, errorCode, validationErrors, validationDetails)
        {
            Value = value;
        }

        // Factory methods
        public static ServiceResult<T> Success(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null in a success result.");
            return new ServiceResult<T>(value, true);
        }

        public static ServiceResult<T> Failure(string errorMessage, string errorCode = null, List<string> validationErrors = null, Dictionary<string, string> validationDetails = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));

            return new ServiceResult<T>(default, false, errorMessage, errorCode, validationErrors, validationDetails);
        }

        // Fluent methods
        public ServiceResult<T> AddValidationError(string error)
        {
            base.AddValidationError(error);
            return this;
        }

        public ServiceResult<T> AddValidationDetail(string fieldName, string errorDetail)
        {
            base.AddValidationDetail(fieldName, errorDetail);
            return this;
        }

        // Transformations
        public ServiceResult<TU> Map<TU>(Func<T, TU> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return IsSuccess
                ? ServiceResult<TU>.Success(mapper(Value))
                : ServiceResult<TU>.Failure(ErrorMessage, ErrorCode, ValidationErrors as List<string>, ValidationDetails);
        }

        public async Task<ServiceResult<TU>> BindAsync<TU>(Func<T, Task<ServiceResult<TU>>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return IsSuccess ? await binder(Value) : ServiceResult<TU>.Failure(ErrorMessage, ErrorCode, ValidationErrors as List<string>, ValidationDetails);
        }

        // Implicit conversions

        public static ServiceResult ToServiceResult(ServiceResult<T> result)
        {
            return result.IsSuccess
                ? Success()
                : Failure(result.ErrorMessage, result.ErrorCode, result.ValidationErrors as List<string>, result.ValidationDetails);
        }

        public static ServiceResult<T> FromServiceResult(ServiceResult result)
        {
            return result.IsSuccess
                ? Success(default)
                : Failure(result.ErrorMessage, result.ErrorCode, result.ValidationErrors as List<string>, result.ValidationDetails);
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Success: {Value}"
                : base.ToString();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Value), Value);
        }
    }
}
