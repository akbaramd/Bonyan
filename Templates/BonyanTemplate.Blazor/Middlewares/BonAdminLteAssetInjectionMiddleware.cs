using System.Text;
using Microsoft.AspNetCore.Http;

namespace Bonyan.AspNetCore.Components.Admin.Middlewares;

public class BonAdminLteAssetInjectionMiddleware
{
    private readonly RequestDelegate _next;

    public BonAdminLteAssetInjectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Intercept only the initial HTML page request
        if (context.Request.Path == "/" || context.Request.Path == "/index.html" || context.Request.Path == "/_Host")
        {
            // Capture the original response body
            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                // Proceed down the middleware pipeline
                await _next(context);

                // Read the response stream
                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                // Inject assets into the <head> section
                var headInjection = @"
    <!-- Google Font: Source Sans Pro -->
    <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,400i,700&display=fallback"">
    <!-- Font Awesome Icons -->
    <link rel=""stylesheet"" href=""_content/Bonyan.AdminLTE.Blazor/plugins/fontawesome-free/css/all.min.css"">
    <!-- Theme style -->
    <link rel=""stylesheet"" href=""_content/Bonyan.AdminLTE.Blazor/dist/css/adminlte.min.css"">
    <!-- RTL style -->
    <link rel=""stylesheet"" href=""_content/Bonyan.AdminLTE.Blazor/dist/css/rtl.css"">
    ";

                responseBody = responseBody.Replace("</head>", $"{headInjection}\n</head>");

                // Inject scripts before blazor.web.js
                var scriptInjection = @"
    <!-- jQuery -->
    <script src=""_content/Bonyan.AdminLTE.Blazor/plugins/jquery/jquery.min.js""></script>
    <!-- Bootstrap 4 -->
    <script src=""_content/Bonyan.AdminLTE.Blazor/plugins/bootstrap/js/bootstrap.bundle.min.js""></script>
    <!-- AdminLTE App -->
    <script src=""_content/Bonyan.AdminLTE.Blazor/dist/js/adminlte.min.js""></script>
    ";

                responseBody = responseBody.Replace(
                    "<script src=\"_framework/blazor.web.js\"></script>",
                    $"{scriptInjection}\n<script src=\"_framework/blazor.web.js\"></script>");

                // Remove the Content-Length header
                context.Response.Headers.ContentLength = null;

                // Reset the response body and write the modified content
                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(responseBody, Encoding.UTF8);
            }
            finally
            {
                // Ensure the original response body is restored
                context.Response.Body = originalBodyStream;
            }
        }
        else
        {
            // For other requests, proceed as normal
            await _next(context);
        }
    }
}