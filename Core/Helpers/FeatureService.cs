using FeatureHubSDK;

namespace Core.Helpers;

public static class FeatureService
{
    public static IClientContext featurehub;

    static FeatureService()
    {
        FeatureLogging.DebugLogger += (sender, s) => MonitorService.Log.Debug($"[FeatureHub @ Authentication Service] DEBUG, {s}");
        FeatureLogging.TraceLogger += (sender, s) => MonitorService.Log.Verbose($"[FeatureHub @ Authentication Service] TRACE, {s}");
        FeatureLogging.InfoLogger += (sender, s) => MonitorService.Log.Information($"[FeatureHub @ Authentication Service] INFO, {s}");
        FeatureLogging.ErrorLogger += (sender, s) => MonitorService.Log.Error($"[FeatureHub @ Authentication Service] ERROR, {s}");

        var config = new EdgeFeatureHubConfig("http://featurehub:8085", "API-KEY");
        featurehub = config.NewContext().Build().Result;

        // FeatureService.featurehub["KEY"].IsEnabled;
    }
}