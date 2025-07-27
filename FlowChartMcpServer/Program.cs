using Avalonia;
using FlowChartMcpServer.Tools;
 using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FlowChartMcpServer;

internal class Program
{
    public static IServiceProvider GlobalServiceProvider { get; set; }

    public static CancellationTokenSource EndSignal { get; set; } = new();
    
    [STAThread]
    public static async Task Main(
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

        var webApp = builder.Build();
        GlobalServiceProvider = webApp.Services;

        // Configure the HTTP request pipeline.
        if (webApp.Environment.IsDevelopment())
            webApp.MapOpenApi();

        if (webApp.Environment.IsProduction())
            webApp.UseHttpsRedirection();

        webApp.MapMcp();
        _ = webApp.RunAsync(EndSignal.Token);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        await EndSignal.CancelAsync();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure(() => new App(GlobalServiceProvider))
                                                             .UsePlatformDetect()
                                                             .WithInterFont()
                                                             .LogToTrace();
}
