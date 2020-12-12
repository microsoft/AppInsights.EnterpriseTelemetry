namespace AppInsights.EnterpriseTelemetry.Context
{
    public class DependencyContextMetadata
    {
        public string DependencyName { get; set; }
        public string TargetSystemName { get; set; }
        public string DependencyType { get; set; }
        public string RequestDetails { get; set; }
        public bool ShouldLogPerformance { get; set; }
    }
}
