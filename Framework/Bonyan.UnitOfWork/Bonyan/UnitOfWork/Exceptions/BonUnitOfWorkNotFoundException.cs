using Bonyan.Exceptions;

namespace Bonyan.UnitOfWork.Exceptions;

public class BonUnitOfWorkNotFoundException(string message = "UnitOfWork can not be found") : BonException(message);