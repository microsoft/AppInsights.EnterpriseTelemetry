namespace AppInsights.EnterpriseTelemetry.Context
{
    /// <summary>
    /// Log Context for logging messages
    /// </summary>
    public class MessageContext: LogContext
    {
        public string Message { get; set; }

        public MessageContext(): base() { }

        public MessageContext(string message, TraceLevel traceLevel, string correlationId, string transactionId, string source, string userId, string e2eId)
            :base(traceLevel, correlationId, transactionId, source, userId, e2eId)
        {
            Message = message;
        }

        public MessageContext(string message)
            :this(message, TraceLevel.Verbose, null, null, null, "System", null)
        {
            Message = message;
        }
    }
}
