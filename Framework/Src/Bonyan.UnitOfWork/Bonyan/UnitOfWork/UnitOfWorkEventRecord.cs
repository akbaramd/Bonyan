namespace Bonyan.UnitOfWork;

public class UnitOfWorkEventRecord
{
    public object EventData { get; }

    public Type EventType { get; }

    public long EventOrder { get; }

    public bool UseOutbox { get; }

    /// <summary>
    /// Extra properties can be used if needed.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = new();

    public UnitOfWorkEventRecord(
        Type eventType,
        object eventData,
        long eventOrder,
        bool useOutbox = true)
    {
        EventType = eventType;
        EventData = eventData;
        EventOrder = eventOrder;
        UseOutbox = useOutbox;
    }
}
