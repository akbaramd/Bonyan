using Bonyan.AspNetCore.Jobs;
using Figgle;
using Hangfire;

namespace Microsoft.AspNetCore.Builder;

public class BonyanApplication(WebApplication application, BonyanServiceInfo serviceInfo)
{
    public WebApplication Application => application;
    public BonyanServiceInfo ServiceInfo { get; } = serviceInfo;

    // List to store information about registered extensions
    public List<ConsoleMessage> ConsoleMessages { get; set; } = new();

    public static IBonyanApplicationBuilder CreateApplicationBuilder(
      string[] args)
    {
      var applicationBuilder = WebApplication.CreateBuilder(args);
    


      // Register the IOptimumJobsManager with the DI container
      applicationBuilder.Services.AddSingleton<IOptimumJobsManager, HangfireJobManager>();


      var  builder = new BonyanApplicationBuilder(new BonyanServiceInfo("", "", ""), applicationBuilder);
      

      return builder;
    }


    public void Run()
    {
        PrintBanner();
        application.Run();
    }

    public async Task RunAsync()
    {
        await application.RunAsync();
    }

    private void PrintBanner()
    {
        // Generate the "Bonyan" banner using Figgle
        var banner = FiggleFonts.Standard.Render("Bonyan");

        // Print the advanced banner
        Console.WriteLine("===============================================================");
        Console.WriteLine(banner);  // Print the Figgle-generated ASCII art for "Bonyan"
        Console.WriteLine("===============================================================");
        Console.WriteLine("                  SERVICE INFORMATION");
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine($"Name     : {ServiceInfo.Name}");
        Console.WriteLine($"ID       : {ServiceInfo.Id}");
        Console.WriteLine($"Version  : {ServiceInfo.Version}");
        Console.WriteLine("===============================================================\n");

        // Print registered extensions
        if (ConsoleMessages.Count > 0)
        {
          Console.WriteLine("Messages:");
          foreach (var msg in ConsoleMessages)
          {
            Console.WriteLine($"[{msg.Category}] {msg.Message} (at {msg.Timestamp})");
          }
          Console.WriteLine("===============================================================\n");
        }
    }
}
