namespace Bonyan.Exceptions;

public class BonException : Exception
{
    public BonException(string message ): base(message:message) { }

    public BonException(string? message , Exception? innerMessage):
        base(message:message , innerException:innerMessage) { }
    
}