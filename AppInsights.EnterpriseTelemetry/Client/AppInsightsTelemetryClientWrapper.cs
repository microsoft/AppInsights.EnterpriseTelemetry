using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public class AppInsightsTelemetryClientWrapper: IAppInsightsTelemetryClientWrapper
    {
        public TelemetryClient Client { get; private set; }

        public AppInsightsTelemetryClientWrapper(TelemetryClient client)
        {
            Client = client;
        }

        public void TrackEvent(EventTelemetry eventTelemetry)
        {
            Client.TrackEvent(eventTelemetry);
        }

        public void TrackException(ExceptionTelemetry exceptionTelemetry)
        {
            Client.TrackException(exceptionTelemetry);
        }

        public void TrackMetric(MetricTelemetry metricTelemetry)
        {
            Client.TrackMetric(metricTelemetry);
        }

        public void TrackTrace(TraceTelemetry traceTelemetry)
        {
            Client.TrackTrace(traceTelemetry);
        }

        public void TrackDependency(DependencyTelemetry dependencyTelemetry)
        {
            Client.TrackDependency(dependencyTelemetry);
        }

        public void TrackRequest(RequestTelemetry requestTelemetry)
        {
            Client.TrackRequest(requestTelemetry);
        }
    }
}
