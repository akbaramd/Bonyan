namespace Microsoft.AspNetCore.Builder;

public class ConsoleMessage
{
  public string Message { get; set; }
  public string Category { get; set; } // e.g., "Extension", "Warning", etc.
  public DateTime Timestamp { get; set; } = DateTime.Now;

  public ConsoleMessage(string message, string category)
  {
    Message = message;
    Category = category;
  }
}