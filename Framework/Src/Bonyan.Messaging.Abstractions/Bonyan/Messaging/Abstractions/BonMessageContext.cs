namespace Bonyan.Messaging.Abstractions;

public class BonMessageContext<TMessage> where TMessage : class
{
    public TMessage Message { get; }
    public IDictionary<string, string> Headers { get; }
    public string CorrelationId { get; }
    public string SenderService { get; }
    public string ReciverService { get; }

    public BonMessageContext(
        TMessage message,
        IDictionary<string, string> headers,
        string correlationId,
        string senderService, string reciverService)
    {
        Message = message;
        Headers = headers;
        CorrelationId = correlationId;
        SenderService = senderService;
        ReciverService = reciverService;
    }

   
}