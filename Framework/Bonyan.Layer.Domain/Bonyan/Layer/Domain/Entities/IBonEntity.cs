namespace Bonyan.Layer.Domain.Entities;

public interface IBonEntity
{
    object[] GetKeys();
}

public interface IBonEntity<TKey> : IBonEntity
{
    public TKey Id { get; set; }
}