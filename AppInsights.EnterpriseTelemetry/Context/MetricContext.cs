namespace AppInsights.EnterpriseTelemetry.Context
{
    public class MetricContext: LogContext
    {
        public string MetricName { get; set; }
        public double Value { get; set; }

        public MetricContext(): base() { }

        public MetricContext(string metricName, double value, string correlationId, string transactionId, string source, string userId, string e2eTrackingId)
            : base(TraceLevel.Metric, correlationId, transactionId, source, userId, e2eTrackingId)
        {
            MetricName = metricName;
            Value = value;
        }

        public MetricContext(string metricName, double value)
            :this(metricName, value, null, null, null, "System", null)
        { }
    }
}
