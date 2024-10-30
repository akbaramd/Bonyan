namespace Bonyan.Exceptions;

public class BonyanException : Exception
{
    public BonyanException(string message ): base(message:message) { }

    public BonyanException(string? message , Exception? innerMessage):
        base(message:message , innerException:innerMessage) { }
    
}