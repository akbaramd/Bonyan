namespace Bonyan.Tracing;


public class BonCorrelationIdOptions
{
    public string HttpHeaderName { get; set; } = "X-Correlation-Id";

    public bool SetResponseHeader { get; set; } = true;
}