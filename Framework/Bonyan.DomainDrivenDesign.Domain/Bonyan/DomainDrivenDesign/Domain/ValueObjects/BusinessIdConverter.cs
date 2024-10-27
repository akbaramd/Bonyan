using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Bonyan.DomainDrivenDesign.Domain.ValueObjects
{
  public class BusinessIdConverter<T> : ValueConverter<T, Guid> where T : BusinessId<T>, new()
  {
    public BusinessIdConverter() 
      : base(
        id => id.Value, // Convert BusinessId<T> to string for database storage
        str => (T)BusinessId<T>.FromGuid(str)) // Convert string back to BusinessId<T>
    {
    }
  }
}
