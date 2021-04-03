using System;
using System.Net;
using AppInsights.EnterpriseTelemetry.Context;

namespace AppInsights.EnterpriseTelemetry.Exceptions
{
    /// <summary>
    /// Base exception for all Application Specific Exception
    /// </summary>
    [Serializable]
    public abstract class BaseAppException : Exception
    {
        /// <summary>
        /// Exception Type (General/Domain/Infrastructure)
        /// </summary>
        public abstract string Type { get; }
        public string DisplayMessage => CreateDisplayMessage();
        
        /// <summary>
        /// Name of the Exception
        /// </summary>
        public virtual string Name { get; protected set; }

        public string CorrelationId { get; protected set; }
        public string TransactionId { get; protected set; }
        public string ExceptionCode { get; protected set; }
        public string ExceptionSource { get; protected set; }
        public HttpStatusCode ResponseCode { get; protected set; }
        public bool LogExceptionMetric { get; protected set; } = false;
        public string DebugHint { get; protected set; }

        /// <summary>
        /// Default constructor for exception
        /// </summary>
        protected BaseAppException() : base() { }

        protected BaseAppException(string message,
            Exception innerException = null,
            string correlationId = "",
            string transactionId = "",
            string exceptionCode = "",
            string source = "",
            HttpStatusCode responseCode = HttpStatusCode.InternalServerError,
            string debugHint = "No debug hints are available")
            : base(message, innerException)
        {
            CorrelationId = correlationId;
            TransactionId = transactionId;
            ExceptionCode = exceptionCode;
            ExceptionSource = source;
            ResponseCode = responseCode;
            DebugHint = debugHint;
        }

        public virtual ExceptionContext CreateLogContext()
        {
            var exceptionContext = new ExceptionContext(this);
            exceptionContext.AddProperty("Exception Type", Name);
            exceptionContext.AddProperty("Exception Code", ExceptionCode);
            exceptionContext.AddProperty("Exception Source", ExceptionSource);
            exceptionContext.AddProperty("Debug Hint", DebugHint);
            return exceptionContext;
        }

        protected abstract string CreateDisplayMessage();
    }
}
