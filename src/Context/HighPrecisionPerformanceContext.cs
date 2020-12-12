using System.Diagnostics;

namespace AppInsights.EnterpriseTelemetry.Context
{
    public class HighPrecisionPerformanceContext: MetricContext
    {
        private readonly Stopwatch _stopwatch;
        public HighPrecisionPerformanceContext(string name, string correlationId = "", string transactionId = "", string source = "", string userId = "", string e2eTrackingId = "")
            : base(name, 0.0, correlationId, transactionId, source, userId, e2eTrackingId)
        {
            _stopwatch = new Stopwatch();
            Start();

        }

        public void Start()
        {
            if (_stopwatch.IsRunning)
                _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            Value = _stopwatch.ElapsedMilliseconds;
            AddProperty("MetricType", "Performance", overridePrevious: true);
        }

        public long GetEllapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;
    }
}
