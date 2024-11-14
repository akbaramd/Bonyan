namespace Bonyan.Layer.Domain.Exceptions;

public class BonDomainException(
    string message = "Error Accord on Application Layer",
    string errorCode = "Global",
    object? parameters = null)
    : Exception(message)
{
    public string ErrorCode { get; set; } = errorCode;
    public object? Parameters { get; set; } = parameters;
}