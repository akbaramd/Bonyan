using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bonyan.Layer.Domain.ValueObjects
{
    
    
    public class BonBusinessIdConverter<T,TKey> : ValueConverter<T, TKey> where T : BonBusinessId<T,TKey>, new()
    {
        public BonBusinessIdConverter()
            : base(
                id => id.Value, // Convert BusinessId<T> to string for database storage
                str => (T)BonBusinessId<T,TKey>.FromValue(str)) // Convert string back to BusinessId<T>
        {
        }
    }
    
    public class BonBusinessIdConverter<T> :BonBusinessIdConverter<T,Guid> where T : BonBusinessId<T>, new()
    {} 

}