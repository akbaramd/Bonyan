namespace Bonyan.Layer.Domain.Entity;

public interface IBonEntity
{
    object GetKey();
}

public interface IBonEntity<TKey> : IBonEntity
{
    public TKey Id { get; set; }
}