using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using AppInsights.EnterpriseTelemetry.Configurations;

#pragma warning disable CA1031 // Do not catch general exception types
namespace AppInsights.EnterpriseTelemetry.AppInsightsInitializers
{
    public class RequestResponseInitializer : ITelemetryInitializer
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationInsightsConfiguration _configuration;

        public RequestResponseInitializer(IHttpContextAccessor httpContextAccessor, ApplicationInsightsConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public RequestResponseInitializer(ApplicationInsightsConfiguration configuration)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _configuration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                if (!(telemetry is RequestTelemetry))
                {
                    return;
                }

                var context = GetHttpContext();
                var requestProperties = GetLogPropertiesFromRequest(context.Request);
                var responseProperties = GetLogPropertiesFromResponse(context.Response);

                ((ISupportProperties)telemetry).Properties.Copy(requestProperties);
                ((ISupportProperties)telemetry).Properties.Copy(responseProperties);
            }
            catch(Exception exception)
            {
                ((ISupportProperties)telemetry).Properties.Add("REQUEST_RESPONSE_INITIALIZER_EXCEPTION", exception.ToString());
            }
        }

        private Dictionary<string, string> GetLogPropertiesFromRequest(HttpRequest request)
        {
            var logProperties = new Dictionary<string, string>();

            foreach (var header in request.Headers)
            {
                if (_configuration.RedactedHeaders.Contains(header.Key))
                    logProperties.AddOrUpdate($"Request:Header:{header.Key}", TelemetryConstant.REDACTED);
                else
                    logProperties.AddOrUpdate($"Request:Header:{header.Key}", string.Join(",", header.Value));
            }
            logProperties.AddOrUpdate("Request:Method", request.Method);
            logProperties.AddOrUpdate("Request:Protocol", request.Protocol);
            logProperties.AddOrUpdate("Request:Scheme", request.Scheme);
            logProperties.AddOrUpdate("Request:Host", request.Host.Value);
            logProperties.AddOrUpdate("Request:Path", request.Path.Value);
            logProperties.AddOrUpdate("Request:QueryString", request.QueryString.HasValue ? request.QueryString.Value : string.Empty);
            return logProperties;
        }

        private Dictionary<string, string> GetLogPropertiesFromResponse(HttpResponse response)
        {
            var logProperties = new Dictionary<string, string>();

            foreach (var header in response.Headers)
            {
                logProperties.AddOrUpdate($"Response:Header:{header.Key}", string.Join(",", header.Value));
            }
            logProperties.AddOrUpdate("Response:StatusCode", response.StatusCode.ToString());
            return logProperties;
        }

        private HttpContext GetHttpContext()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // Trying to re-initialize the HTTP Context accessor
                _httpContextAccessor = new HttpContextAccessor();
            }

            if (_httpContextAccessor.HttpContext == null)
                return null;

            return _httpContextAccessor.HttpContext;
        }
    }
}
#pragma warning restore CA1031 // Do not catch general exception types
