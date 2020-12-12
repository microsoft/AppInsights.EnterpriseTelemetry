namespace AppInsights.EnterpriseTelemetry.Context.Background
{
    public class ContextDetails
    {
        public string ExecutingId { get; set; }
        public string OperationName { get; set; }
        public string EndToEndTrackingId { get; set; }
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
    }
}
