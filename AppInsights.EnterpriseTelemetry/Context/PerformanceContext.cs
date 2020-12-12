using System;

namespace AppInsights.EnterpriseTelemetry.Context
{
    public class PerformanceContext: MetricContext
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public PerformanceContext(): base() { }

        public PerformanceContext(string name, string correlationId, string transactionId, string source, string userId, string e2eTrackingId)
            :base(name, 0.0, correlationId, transactionId, source, userId, e2eTrackingId)
        {
            StartTime = DateTime.UtcNow;
            EndTime = DateTime.UtcNow;
        }

        public PerformanceContext(string name)
            :this(name, null, null, null, "System", null)
        { }

        public void Start()
        {
            StartTime = DateTime.UtcNow;
        }

        public void Stop()
        {
            EndTime = DateTime.UtcNow;
            var timeTaken = (EndTime - StartTime).TotalMilliseconds;
            Value = timeTaken;
            AddProperty("MetricType", "Performance", overridePrevious: true);
            AddProperty(nameof(StartTime), StartTime.ToString());
            AddProperty(nameof(EndTime), EndTime.ToString());
        }

        public long GetEllapsedMilliseconds() => (long)(EndTime - StartTime).TotalMilliseconds;
        
        public double GetCurrentEllapsedMilliSeconds => (DateTime.UtcNow - StartTime).TotalMilliseconds;
    }
}
