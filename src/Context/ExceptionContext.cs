using System;
using AppInsights.EnterpriseTelemetry.Configurations;
using AppInsights.EnterpriseTelemetry.Exceptions;

namespace AppInsights.EnterpriseTelemetry.Context
{
    /// <summary>
    /// Context for logging exceptions in the application
    /// </summary>
    public class ExceptionContext : LogContext
    {
        public Exception Exception { get; set; }

        public bool ExceptionTrimmingEnabled { get; set; }

        public ExceptionContext() : base() { }

        /// <summary>
        /// Constructs the exception context for a custom application exception
        /// </summary>
        /// <param name="appException" cref="BaseAppException">App Exception thrown in the application</param>
        /// <param name="severityLevel" cref="TraceLevel">Severity level of the exception</param>
        /// <param name="userId">ID of the user facing the exception</param>
        public ExceptionContext(BaseAppException appException, TraceLevel severityLevel, string userId)
            : base(severityLevel, appException.CorrelationId, appException.TransactionId, appException.ExceptionSource, userId, null)
        {
            Exception = appException;
        }

        public ExceptionContext(BaseAppException appException)
            : this(appException, TraceLevel.Error, "System")
        {
            Exception = appException;
        }

        public ExceptionContext(Exception exception, TraceLevel traceLevel, string correlationId, string transactionId, string source, string userId, string e2eId)
            : base(traceLevel, correlationId, transactionId, source, userId, e2eId)
        {
            Exception = exception;
        }

        public ExceptionContext(Exception exception)
            : this(exception, TraceLevel.Error, null, null, null, "System", null)
        { }

        public override void Trim(ApplicationInsightsConfiguration configuration)
        {   
            if (!(configuration.ExceptionTrimmingEnabled || ExceptionTrimmingEnabled))
                return;

            Exception = TrimException(Exception, 1, configuration);
            base.Trim(configuration);
            AddProperty("Trimming", "Enabled");
        }

        private Exception TrimException(Exception exception, int currentDepth, ApplicationInsightsConfiguration configuration)
        {
            bool isTrimminRequired = IsTrimmingRequired(exception, currentDepth, configuration);
            if (!isTrimminRequired)
                return exception;

            Exception trimmedException = new Exception(
                message: exception.Message.Substring(0, exception.Message.Length <= configuration.MaxMessageSize ? exception.Message.Length : configuration.MaxMessageSize),
                innerException: TrimException(exception.InnerException, currentDepth + 1, configuration))
            {
                Source = exception.Source,
                HelpLink = exception.HelpLink
            };

            AddProperty($"StackTrace_{currentDepth}", exception.StackTrace);
            AddProperty($"TargetSite_{currentDepth}", exception.TargetSite);
            AddProperty($"ExceptionSource_{currentDepth}", exception);
            return trimmedException;
        }

        private bool IsTrimmingRequired(Exception exception, int currentDepth, ApplicationInsightsConfiguration configuration)
        {
            bool isOutOfDepth = currentDepth >= configuration.MaxExceptionDepth;
            bool isMessageOutOfBounds = exception.Message.Length > configuration.MaxMessageSize;
            return isOutOfDepth || isMessageOutOfBounds;
        }
    }
}
