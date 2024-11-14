namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonEntity
{
    object[] GetKeys();
}

public interface IBonEntity<TKey> : IBonEntity
{
    public TKey Id { get; set; }
}