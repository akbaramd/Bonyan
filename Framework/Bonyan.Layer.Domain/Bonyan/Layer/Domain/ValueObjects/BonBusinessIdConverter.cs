using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bonyan.Layer.Domain.ValueObjects
{
  public class BonBusinessIdConverter<T> : ValueConverter<T, Guid> where T : BonBusinessId<T>, new()
  {
    public BonBusinessIdConverter() 
      : base(
        id => id.Value, // Convert BusinessId<T> to string for database storage
        str => (T)BonBusinessId<T>.FromGuid(str)) // Convert string back to BusinessId<T>
    {
    }
  }
}
