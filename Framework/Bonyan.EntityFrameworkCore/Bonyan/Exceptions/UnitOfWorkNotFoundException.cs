namespace Bonyan.Exceptions;

public class UnitOfWorkNotFoundException(string message = "UnitOfWork can not be found") : BonyanException(message);