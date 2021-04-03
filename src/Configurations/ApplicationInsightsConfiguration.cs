using System.Collections.Generic;
using Microsoft.ApplicationInsights.Extensibility;
using static AppInsights.EnterpriseTelemetry.TelemetryConstant;

namespace AppInsights.EnterpriseTelemetry.Configurations
{
    public class ApplicationInsightsConfiguration
    {
        public string InstrumentationKey { get; set; }
        public TraceLevel LogLevel { get; set; }

        private string _transactionIdPropertyKey;
        public string TransactionIdPropertyKey { get => !string.IsNullOrWhiteSpace(_transactionIdPropertyKey) ? _transactionIdPropertyKey : TRANSACTION_KEY; set => _transactionIdPropertyKey = value; }

        private string _correlationIdPropertyKey;
        public string CorrelationIdPropertyKey { get => !string.IsNullOrWhiteSpace(_correlationIdPropertyKey) ? _correlationIdPropertyKey : CORRELATION_KEY; set => _correlationIdPropertyKey = value; }

        private string _subCorrelationIdPropertyKey;
        public string SubCorrelationIdPropertyKey { get => !string.IsNullOrWhiteSpace(_subCorrelationIdPropertyKey) ? _subCorrelationIdPropertyKey : SUB_CORRELATION_KEY; set => _subCorrelationIdPropertyKey = value; }

        private string _tenantIdPropertyKey;
        public string TenantIdPropertyKey { get => !string.IsNullOrWhiteSpace(_tenantIdPropertyKey) ? _tenantIdPropertyKey : TENANT_KEY; set => _tenantIdPropertyKey = value; }

        private string _endToEndPropertyKey;
        public string EndToEndIdPropertyKey { get => !string.IsNullOrWhiteSpace(_endToEndPropertyKey) ? _endToEndPropertyKey : E2E_KEY; set => _endToEndPropertyKey = value; }

        private string _userPropertyKey;
        public string UserPropertyKey { get => !string.IsNullOrWhiteSpace(_userPropertyKey) ? _userPropertyKey : USER_KEY; set => _userPropertyKey = value; }

        private string _businessProcessPropertyKey;
        public string BusinessProcessPropertyKey { get => !string.IsNullOrWhiteSpace(_businessProcessPropertyKey) ? _businessProcessPropertyKey : BUSINESS_PROCESS_KEY; set => _businessProcessPropertyKey = value; }

        public Dictionary<string, string> CustomTrackingProperties { get; set; } = new Dictionary<string, string>();

        public bool EnvironmentInitializerEnabled { get; set; }
        public bool ClientSideErrorSuppressionEnabled { get; set; }
        public bool AutoTrackingEnabled { get; set; }
        public bool ResponseCodeTranslationEnabled { get; set; }
        public bool ResponseBodyTrackingEnabled { get; set; }
        public bool RequestBodyTrackingEnabled { get; set; }
        public bool RequestTelemetryEnhanced { get; set; }

        public List<ITelemetryInitializer> CustomInitializers { get; } = new List<ITelemetryInitializer>();

        public int MaxPropertySize { get; set; }
        public int MaxMessageSize { get; set; }
        public int MaxExceptionDepth { get; set; }

        public bool PropertySplittingEnabled { get; set; }
        public bool ExceptionTrimmingEnabled { get; set; }
    }
}
