using System;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using AppInsights.EnterpriseTelemetry.Client;
using AppInsights.EnterpriseTelemetry.Context;
using AppInsights.EnterpriseTelemetry.Exceptions;
using Microsoft.ApplicationInsights.DataContracts;
using AppInsights.EnterpriseTelemetry.Configurations;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

#pragma warning disable CA1031 // Do not catch general exception types
namespace AppInsights.EnterpriseTelemetry
{
    /// <summary>
    /// Logs data in Azure Application Insights
    /// </summary>
    public class ApplicationInsightsLogger : ILogger
    {
        private readonly IAppInsightsTelemetryClientWrapper _client;
        private readonly IAppInsightsClientManager _clientManager;
        private readonly ApplicationInsightsConfiguration _configuration;
        private readonly IContextPropertyBuilder _contextPropertyBuilder;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary cref="TelemetryClient">
        /// Client for connecting to Application Insights
        /// </summary>
        public TelemetryClient Client => _client.Client;

        public ApplicationInsightsLogger(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration)
        {
            _configuration = applicationInsightsConfiguration;
            _clientManager = new AppInsightsClientManager();
            _contextPropertyBuilder = new HttpContextPropertyBuilder();
            _client = _clientManager.CreateClient(applicationInsightsConfiguration, appMetadataConfiguration);
            _httpContextAccessor = new HttpContextAccessor();
        }

        public ApplicationInsightsLogger(IAppInsightsTelemetryClientWrapper client, ApplicationInsightsConfiguration applicationInsightsConfiguration, IContextPropertyBuilder contextPropertyBuilder, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = applicationInsightsConfiguration;
            _client = client;
            _contextPropertyBuilder = contextPropertyBuilder;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual void Log(string message, string correlationId = "", string transactionId = "", string source = "", string userId = "", string e2eTrackingId = "")
        {
            var messageContext = new MessageContext(message, TraceLevel.Verbose, correlationId, transactionId, source, userId, e2eTrackingId);
            messageContext.Trim(_configuration);
            Log(messageContext);
        }

        public virtual void Log(MessageContext message)
        {
            try
            {
                if (!(ValidateLog(message)))
                    return;
                message.Trim(_configuration);
                var traceTelemetry = new TraceTelemetry(message.Message, GetAppInsightsSevLevel(message.TraceLevel));
                traceTelemetry.Properties.Copy(message.Properties);
                _contextPropertyBuilder.Build(traceTelemetry, message.ContextualProperties);
                _client.TrackTrace(traceTelemetry);
            }
            catch (Exception exception)
            {
                LogInternal(exception);
            }
        }

        public virtual void Log(Exception exception, string correlationId = "", string transactionId = "", string source = "", string userId = "", string e2eTrackingId = "")
        {   
            if (exception is BaseAppException)
            {
                LogAppException(exception as BaseAppException, userId, e2eTrackingId);
            }
            else
            {
                var exceptionContext = new ExceptionContext(exception, TraceLevel.Error, correlationId, transactionId, source, userId, e2eTrackingId);
                Log(exceptionContext);
            }
        }

        private void LogAppException(BaseAppException appException, string userId, string e2eTrackingId)
        {
            var exceptionContext = appException.CreateLogContext();
            exceptionContext.EndToEndTrackingId = e2eTrackingId;
            exceptionContext.AddUserId(userId);
            Log(exceptionContext);

            if (appException.LogExceptionMetric)
            {
                var exceptionMetric = new MetricContext(appException.Name, 1, appException.CorrelationId, appException.TransactionId, appException.ExceptionSource, userId, e2eTrackingId);
                Log(exceptionMetric);
            }
        }

        public virtual void Log(ExceptionContext exception)
        {
            try
            {
                if (!(ValidateLog(exception)))
                    return;

                exception.Trim(_configuration);
                var exceptionTelemetry = new ExceptionTelemetry(exception.Exception)
                {
                    SeverityLevel = GetAppInsightsSevLevel(exception.TraceLevel)
                };
                exceptionTelemetry.Properties.Copy(exception.Properties);
                _contextPropertyBuilder.Build(exceptionTelemetry, exception.ContextualProperties);
                _client.TrackException(exceptionTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        public virtual void Log(EventContext eventContext, params Tuple<string, string>[] additionalProperties)
        {
            try
            {   
                if (!(ValidateLog(eventContext)))
                    return;

                eventContext.Trim(_configuration);
                var eventTelemetry = new EventTelemetry(eventContext.EventName);
                eventTelemetry.Properties.Copy(eventContext.Properties);
                if (additionalProperties != null && additionalProperties.Any())
                {
                    foreach (var property in additionalProperties)
                    {
                        eventTelemetry.Properties.AddOrUpdate(property.Item1, property.Item2);
                    }
                }
                eventTelemetry.Metrics.Copy(eventContext.Metrics);
                _contextPropertyBuilder.Build(eventTelemetry, eventContext.ContextualProperties);
                _client.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        public virtual void Log(EventContext eventContext)
        {
            try
            {
                eventContext.Trim(_configuration);
                if (!(ValidateLog(eventContext)))
                    return;
                var eventTelemetry = new EventTelemetry(eventContext.EventName);
                eventTelemetry.Properties.Copy(eventContext.Properties);
                eventTelemetry.Metrics.Copy(eventContext.Metrics);
                _contextPropertyBuilder.Build(eventTelemetry, eventContext.ContextualProperties);
                _client.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        public void Log(MetricContext metricContext)
        {
            try
            {
                if (!(ValidateLog(metricContext)))
                    return;

                metricContext.Trim(_configuration);
                var metricTelemetry = new MetricTelemetry(metricContext.MetricName, metricContext.Value);
                metricTelemetry.Properties.Copy(metricContext.Properties);
                _contextPropertyBuilder.Build(metricTelemetry, metricContext.ContextualProperties);
                _client.TrackMetric(metricTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        public virtual void Log(DependencyContext dependencyContext)
        {
            try
            {
                dependencyContext.CreateAdditionalProperties();
                dependencyContext.Trim(_configuration);
                var dependencyTelemetry = new DependencyTelemetry(dependencyContext.DependencyType, dependencyContext.TargetSystemName, dependencyContext.DependencyName, dependencyContext.RequestDetails)
                {
                    ResultCode = dependencyContext.ResponseCode,
                    Success = dependencyContext.IsSuccessfull,
                    Duration = dependencyContext.TimeTaken
                };
                dependencyTelemetry.Properties.Copy(dependencyContext.Properties);
                _contextPropertyBuilder.Build(dependencyTelemetry, dependencyContext.ContextualProperties);
                _client.TrackDependency(dependencyTelemetry);

                var exceptionContext = dependencyContext.GetExceptionContext();
                if (exceptionContext != null)
                    Log(exceptionContext);

                var performanceContext = dependencyContext.GetPerformanceContext();
                if (performanceContext != null)
                    Log(performanceContext);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        public virtual void LogRequestBody(object requestBody)
        {
            try
            {
                if (!(_configuration.RequestTelemetryEnhanced && _configuration.RequestBodyTrackingEnabled))
                    return;

                if (_httpContextAccessor == null)
                    return;

                var currentContext = _httpContextAccessor.HttpContext;
                var requestTelemetry = currentContext?.Features.Get<RequestTelemetry>();
                if (requestTelemetry != null)
                {
                    string serializedRequestBody;
                    try
                    {
                        if (requestBody == null)
                            serializedRequestBody = "__EMPTY__";
                        else if (requestBody is string)
                            serializedRequestBody = requestBody as string;
                        else
                            serializedRequestBody = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (Exception exception)
                    {
                        serializedRequestBody = $"Request Body cannot be serialized. Error - {exception}";
                    }
                    requestTelemetry.Properties.AddOrUpdate("Request:Body", serializedRequestBody);
                }
                else
                {
                    var currentUri = currentContext != null ? currentContext.Request.GetUri() : new Uri("https://__missing__");
                    requestTelemetry = new RequestTelemetry()
                    {
                        Url = currentUri
                    };
                    _client.TrackRequest(requestTelemetry);
                }
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        private void LogInternal(Exception exception)
        {
            //Try to log exception in App Insights
            try
            {
                _client.TrackException(new ExceptionTelemetry(exception));
            }
            catch (Exception) //Write message to trace listener
            {
                Debug.Write($"UNHANDLED EXCEPTION IN TELEMETRY: {exception}");

                //Don't rethrow exception
            }
        }

        private SeverityLevel GetAppInsightsSevLevel(TraceLevel traceLevel)
        {
            return traceLevel switch
            {
                TraceLevel.Critical => SeverityLevel.Critical,
                TraceLevel.Error => SeverityLevel.Error,
                TraceLevel.Warning => SeverityLevel.Warning,
                TraceLevel.Verbose => SeverityLevel.Verbose,
                TraceLevel.Information => SeverityLevel.Information,
                TraceLevel.Metric => SeverityLevel.Information,
                _ => SeverityLevel.Information,
            };
        }

        private bool ValidateLog(LogContext context)
        {
            var currentTraceLevel = _configuration.LogLevel;
            if (context.TraceLevel < currentTraceLevel)
                return false;
            return true;
        }
    }
}
#pragma warning restore CA1031 // Do not catch general exception types