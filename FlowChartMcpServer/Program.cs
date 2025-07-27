using Avalonia;
using FlowChartMcpServer.Tools;
 using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FlowChartMcpServer;

internal class Program
{
    [STAThread]
    public static void Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddMcpServer()
               // .WithListToolsHandler(McpRequestHandler.HandleListToolsAsync)
               // .WithCallToolHandler(McpRequestHandler.HandleCallToolAsync)
               .WithTools<FlowChartTools>()
               .WithHttpTransport();

        builder.Services.AddOpenTelemetry()
               .WithTracing(b => b.AddSource("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
               .WithMetrics(b => b.AddMeter("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
               .WithLogging()
               .UseOtlpExporter();

        // DI
        builder.Services.AddSingleton<FlowChartManager>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        if (app.Environment.IsProduction())
            app.UseHttpsRedirection();

        app.MapMcp();
        app.RunAsync();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
