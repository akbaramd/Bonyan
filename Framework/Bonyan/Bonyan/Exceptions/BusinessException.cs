
using Microsoft.Extensions.Logging;

namespace Bonyan.Exceptions;

[Serializable]
public class BusinessException(
  string? code = null,
  string? message = null,
  string? details = null,
  Exception? innerException = null,
  LogLevel logLevel = LogLevel.Warning)
  : BonException(message, innerException),
    IBusinessException,
    IHasErrorCode,
    IHasErrorDetails
{
  public string? Code { get; set; } = code;

  public string? Details { get; set; } = details;

  public LogLevel LogLevel { get; set; } = logLevel;


  public BusinessException WithData(string name, object value)
  {
    Data[name] = value;
    return this;
  }
}
