using System.Linq;
using System.Collections.Generic;

namespace AppInsights.EnterpriseTelemetry.Context
{
    public class EventContext : LogContext
    {
        public string EventName { get; set; }
        public Dictionary<string, double> Metrics { get; set; }
        public bool IsBusinessEvent { get; set; }
        public Dictionary<string, string> BusinessProperties { get; set; }

        public EventContext() : base()
        {
            Metrics = new Dictionary<string, double>();
            BusinessProperties = new Dictionary<string, string>();
        }

        public EventContext(string eventName, string correlationId, string transactionId, string source, string userId, string e2eTrackingId)
            : base(TraceLevel.Metric, correlationId, transactionId, source, userId, e2eTrackingId)
        {
            EventName = eventName;
            Metrics = new Dictionary<string, double>();
            BusinessProperties = new Dictionary<string, string>();
        }

        public EventContext(string eventName, string correlationId, string transactionId)
            : base(TraceLevel.Metric, correlationId, transactionId, "Default", "System", "N/A")
        {
            EventName = eventName;
            Metrics = new Dictionary<string, double>();
            BusinessProperties = new Dictionary<string, string>();
        }

        public EventContext(string eventName)
            : this(eventName, null, null)
        {
            EventName = eventName;
            Metrics = new Dictionary<string, double>();
            BusinessProperties = new Dictionary<string, string>();
        }

        public EventContext(EventContextMetadata metadata)
            : base()
        {
            EventName = $"{metadata.Object}.{metadata.Action}";
            if (!string.IsNullOrWhiteSpace(metadata.Context))
                EventName += $".{metadata.Context}";

            if (!string.IsNullOrWhiteSpace(metadata.Subject))
                AddProperty("Subject", metadata.Subject);

            if (!string.IsNullOrWhiteSpace(metadata.Desription))
                AddProperty("Description", metadata.Desription);

            if (metadata.StaticProperties != null && metadata.StaticProperties.Any())
                AddProperties(metadata.StaticProperties);

            if (metadata.ContextualProperties != null && metadata.ContextualProperties.Any())
                AddContextualProperties(metadata.ContextualProperties);
        }

        public void AddMetric(string metricName, double metricValue, bool overridePrevious)
        {
            if (Metrics.ContainsKey(metricName))
            {
                if (overridePrevious)
                {
                    Metrics[metricName] = metricValue;
                }
            }
            else
            {
                Metrics.Add(metricName, metricValue);
            }
        }

        public void AddMetric(string metricName, double metricValue)
        {
            AddMetric(metricName, metricValue, overridePrevious: false);
        }
    }
}
