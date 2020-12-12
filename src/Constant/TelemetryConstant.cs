namespace AppInsights.EnterpriseTelemetry
{
    public struct TelemetryConstant
    {
        public const string CORRELATION_KEY = "XCV";
        public const string SUB_CORRELATION_KEY = "Sub-XCV";
        public const string TRANSACTION_KEY = "MessageId";
        public const string TENANT_KEY = "TenantId";
        public const string USER_KEY = "LoggedInUserId";
        public const string E2E_KEY = "EndToEndTrackingId";
        public const string BUSINESS_PROCESS_KEY = "BusinessProcessName";

        public const string NA = "N/A";

        public const string REDACTED = "**REDACTED**";
        public const string NO_RESPONSE = "No Response";

        public const string HEADER_DEFAULT_CORRELATION_KEY = "x-correlationid";
        public const string HEADER_DEFAULT_SUB_CORRELATION_KEY = "x-sub-correlationid";
        public const string HEADER_DEFAULT_E2E_KEY = "x-e2e-trackingid";
        public const string HEADER_DEFAULT_TRANSACTION_KEY = "x-messageid";
        public const string HEADER_DEFAULT_TENANT_ID = "x-ms-tenant";
        public const string HEADER_DEFAULT_BUSINESS_PROCESS_NAME = "x-businessprocessname";

        public const string ACTION_URI = "ActionUri";
        public const string APP_ACTION = "AppAction";
        public const string BUSINESS_PROCESS_NAME = "BusinessProcessName";
        public const string USER_ROLE_NAME = "UserRoleName";
        public const string TARGET_ENTITY = "TargetEntityKey";
        public const string SENDER_ID = "SenderId";
        public const string RECEIVER_ID = "ReceiverId";
        public const string COMPONENT_TYPE = "ComponentType";
        public const string START_DATE_TIME = "StartDateTime";
        public const string END_DATE_TIME = "EndDateTime";
        public const string EVENT_OCCURRENCE_TIME = "EventOccurrenceTime";

        public const int MAX_PROPERTY_SIZE = 8192;
        public const int MAX_EXCEPTION_DEPTH = 20;
        public const int MAX_MESSAGE_SIZE = 1000;

        public struct DependencyTypes
        {
            public const string HTTP = "HTTP";
            public const string Redis = "AZURE-REDIS-CACHE";
            public const string CosmosDb = "COSMOS-DB";
            public const string SQL = "SQL";
            public const string WCF = "WCF";
        }
    }
}
