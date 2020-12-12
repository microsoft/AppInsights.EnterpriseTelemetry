using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public interface IAppInsightsTelemetryClientWrapper
    {
        TelemetryClient Client { get; }
        void TrackTrace(TraceTelemetry traceTelemetry);
        void TrackException(ExceptionTelemetry exceptionTelemetry);
        void TrackEvent(EventTelemetry eventTelemetry);
        void TrackMetric(MetricTelemetry metricTelemetry);
        void TrackDependency(DependencyTelemetry dependencyTelemetry);
        void TrackRequest(RequestTelemetry requestTelemetry);
    }
}
