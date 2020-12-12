using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public interface IAppInsightsClientManager
    {
        IAppInsightsTelemetryClientWrapper CreateClient(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration);
    }
}
