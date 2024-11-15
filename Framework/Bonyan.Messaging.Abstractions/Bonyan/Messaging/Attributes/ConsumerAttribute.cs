namespace Bonyan.Messaging.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConsumerServiceAttribute : Attribute
{
    public string ServiceName { get; }

    public ConsumerServiceAttribute(string serviceName)
    {
        ServiceName = serviceName;
    }
}