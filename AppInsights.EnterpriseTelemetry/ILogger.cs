using System;
using AppInsights.EnterpriseTelemetry.Context;

namespace AppInsights.EnterpriseTelemetry
{
    /// <summary>
    /// Application Logger for logging data in given source
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a text messate in Log Database
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="correlationId">Correlation ID of the operation</param>
        /// <param name="transactionId">Transaction ID of the operation</param>
        /// <param name="source">Method there the log is originating from</param>
        /// <param name="userId">Logged In User of the operation</param>
        /// <param name="e2eTrackingId">End-to-end tracking ID</param>
        void Log(string message, string correlationId = "", string transactionId = "", string source = "", string userId = "", string e2eTrackingId = "");

        /// <summary>
        /// Logs an system exception in Log Database
        /// </summary>
        /// <param name="exception" cref="Exception">Exception to be logged</param>
        /// <param name="correlationId">Correlation ID of the operation</param>
        /// <param name="transactionId">Transaction ID of the operation</param>
        /// <param name="source">Method there the log is originating from</param>
        /// <param name="userId">Logged In User of the operation</param>
        /// <param name="e2eTrackingId">End-to-end tracking ID</param>
        void Log(Exception exception, string correlationId = "", string transactionId = "", string source = "", string userId = "", string e2eTrackingId = "");
        
        /// <summary>
        /// Logs an message in the Log Database
        /// </summary>
        /// <param name="mesage" cref="MessageContext">Message details</param>
        void Log(MessageContext mesage);
        
        /// <summary>
        /// Logs an exception in the Log Database
        /// </summary>
        /// <param name="exception" cref="ExceptionContext">Exception details</param>
        void Log(ExceptionContext exception);
        
        /// <summary>
        /// Logs an Custom Event in the Log Database
        /// </summary>
        /// <param name="eventContext" cref="EventContext">Event details</param>
        void Log(EventContext eventContext);
        
        /// <summary>
        /// Logs an Metric in the Log Database
        /// </summary>
        /// <param name="metric" cref="MetricContext">Metric Details</param>
        void Log(MetricContext metric);
        
        /// <summary>
        /// Logs a Dependency call of the system in the Log Database
        /// </summary>
        /// <param name="dependency" cref="DependencyContext">Dependency Details</param>
        void Log(DependencyContext dependency);

        /// <summary>
        /// Logs an event in the Log Database
        /// </summary>
        /// <param name="eventContext" cref="EventContext">Event details</param>
        /// <param name="additionalProperties">Additional properties to be logged</param>
        void Log(EventContext eventContext, params Tuple<string, string>[] additionalProperties);

        /// <summary>
        /// Logs the request body to the current telemetry context
        /// </summary>
        /// <param name="requestBody">Request Body</param>
        void LogRequestBody(object requestBody);
    }
}
