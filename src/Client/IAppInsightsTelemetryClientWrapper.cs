using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public interface IAppInsightsTelemetryClientWrapper
    {
        TelemetryClient Client { get; }
        void TrackTrace(TraceTelemetry traceTelemetry);
        void TrackException(ExceptionTelemetry exceptionTelemetry);
        void TrackException(Exception exception, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);
        void TrackEvent(EventTelemetry eventTelemetry);
        void TrackMetric(MetricTelemetry metricTelemetry);
        void TrackDependency(DependencyTelemetry dependencyTelemetry);
        void TrackRequest(RequestTelemetry requestTelemetry);
    }
}
