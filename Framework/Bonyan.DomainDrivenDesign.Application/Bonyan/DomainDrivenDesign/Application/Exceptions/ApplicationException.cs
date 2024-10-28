using System.Net;

namespace Bonyan.DomainDrivenDesign.Application.Exceptions;

public class ApplicationException(
  HttpStatusCode status,
  string message = "Error Accord on Application Layer",
  string errorCode = "Global",
  object? parameters = null)
  : Exception(message)
{
  public string ErrorCode { get; set; } = errorCode;
  public HttpStatusCode Status { get; set; } = status;
  public object? Parameters { get; set; } = parameters;
}
