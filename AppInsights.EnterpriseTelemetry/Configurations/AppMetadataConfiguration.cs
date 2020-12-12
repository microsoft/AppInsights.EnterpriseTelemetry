namespace AppInsights.EnterpriseTelemetry.Configurations
{
    public class AppMetadataConfiguration
    {   
        public string TenantIdHeaderKey { get; set; }
        public string CorrelationIdHeaderKey { get; set; }
        public string SubCorrIdHeaderKey { get; set; }
        public string TransactionIdHeaderKey { get; set; }
        public string EndToEndTrackingHeaderKey { get; set; }
        public string BusinessProcessHeaderKey { get; set; }
    }
}
