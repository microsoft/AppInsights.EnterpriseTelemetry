using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using AppInsights.EnterpriseTelemetry.Configurations;
using AppInsights.EnterpriseTelemetry.AppInsightsInitializers;
using AppInsights.EnterpriseTelemetry.AppInsightsProcessors;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public class AppInsightsClientManager: IAppInsightsClientManager
    {
        public IAppInsightsTelemetryClientWrapper CreateClient(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration)
        {
            var appInsightsConfiguration = new TelemetryConfiguration(applicationInsightsConfiguration.InstrumentationKey);

            if (applicationInsightsConfiguration.ClientSideErrorSuppressionEnabled)
                appInsightsConfiguration.TelemetryInitializers.Add(new ClientSideErrorInitializer());
            
            if (applicationInsightsConfiguration.AutoTrackingEnabled)
                appInsightsConfiguration.TelemetryInitializers.Add(new TrackingInitializer(applicationInsightsConfiguration, appMetadataConfiguration));
            
            if (applicationInsightsConfiguration.ResponseCodeTranslationEnabled)
                appInsightsConfiguration.TelemetryInitializers.Add(new ResponseCodeTranslationIntitializer());

            if (applicationInsightsConfiguration.RequestTelemetryEnhanced)
                appInsightsConfiguration.TelemetryInitializers.Add(new RequestResponseInitializer(applicationInsightsConfiguration));

            if (applicationInsightsConfiguration.CustomInitializers != null && applicationInsightsConfiguration.CustomInitializers.Any())
            {
                foreach(var customInitializer in applicationInsightsConfiguration.CustomInitializers)
                {
                    appInsightsConfiguration.TelemetryInitializers.Add(customInitializer);
                }
            }

            appInsightsConfiguration.TelemetryProcessorChainBuilder.Use((next) => new ExcludedRequestsFilter(next, applicationInsightsConfiguration));

            var client = new TelemetryClient(appInsightsConfiguration)
            {
                InstrumentationKey = applicationInsightsConfiguration.InstrumentationKey
            };
            var wrapper = new AppInsightsTelemetryClientWrapper(client);
            return wrapper;
        }
    }
}
