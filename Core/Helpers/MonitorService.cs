using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using System.Diagnostics;
using System.Reflection;

namespace Core.Helpers;

public static class MonitorService
{
    // ZIPKIN - Activity and Tracer
    public static readonly string ServiceName = Assembly.GetCallingAssembly().GetName().Name ?? "Unknown";
    public static TracerProvider TracerProvider;
    public static ActivitySource ActivitySource = new ActivitySource(ServiceName, "1.0.0");

    // SERILOG - Logger
    public static ILogger Log => Serilog.Log.Logger;

    static MonitorService()
    {
        // OpenTelemetry (ZIPKIN) - Configuration
        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddConsoleExporter()
            .AddZipkinExporter(config =>
            {
                config.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
            })
            .AddSource(ActivitySource.Name)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
            .SetSampler(new AlwaysOnSampler())
            .Build();

        // SERILOG - Configuration
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithSpan()
            .WriteTo.Seq("http://seq:5341")
            .WriteTo.Console()
            .CreateLogger();
    }
}
