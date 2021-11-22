using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.AppInsightsProcessors
{
    /// <summary>
    /// Processor to filter requests that should be filtered from logging (e.g. Health probes and synthetic transactions)
    /// </summary>
    public class ExcludedRequestsFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationInsightsConfiguration _configuration;

        public ExcludedRequestsFilter(ITelemetryProcessor next, IHttpContextAccessor httpContextAccecssor, ApplicationInsightsConfiguration configuration)
        {
            Next = next;
            _httpContextAccessor = httpContextAccecssor;
            _configuration = configuration;
        }

        public ExcludedRequestsFilter(ITelemetryProcessor next, ApplicationInsightsConfiguration configuration)
        {
            Next = next;
            _httpContextAccessor = new HttpContextAccessor();
            _configuration = configuration;
        }

        public void Process(ITelemetry item)
        {
            bool shouldBlockProcessing = ShouldBlockProcessing();
            if (shouldBlockProcessing)
                return;

            Next.Process(item);
        }

        private bool ShouldBlockProcessing()
        {
            HttpContext httpContext = GetHttpContext();
            if (httpContext == null || httpContext.Request == null)
                return false;

            return IsRequestUrlExcluded(httpContext) || IsRequestHeaderExcluded(httpContext);
        }

        private bool IsRequestUrlExcluded(HttpContext httpContext)
        {
            if (_configuration.ExcludedRequestUrls == null && !_configuration.ExcludedRequestUrls.Any())
                return false;

            StringBuilder requestFormatBuilder = new();
            requestFormatBuilder.Append(httpContext.Request.Method);
            requestFormatBuilder.Append(" ");
            requestFormatBuilder.Append(httpContext.Request.Path.Value);
            string requestFormat = requestFormatBuilder.ToString();

            return _configuration.ExcludedRequestUrls.Any(excludedUrl => excludedUrl.ToLowerInvariant() == requestFormat.ToLowerInvariant());
        }

        private bool IsRequestHeaderExcluded(HttpContext httpContext)
        {
            if (_configuration.ExcludedRequestHeaders == null && !_configuration.ExcludedRequestHeaders.Any())
                return false;

            if (httpContext.Request.Headers == null || !httpContext.Request.Headers.Any())
                return false;

            foreach (KeyValuePair<string, string> excludedHeader in _configuration.ExcludedRequestHeaders)
            {
                if (httpContext.Request.Headers.ContainsKey(excludedHeader.Key))
                {
                    if (excludedHeader.Value.ToLowerInvariant() == httpContext.Request.Headers[excludedHeader.Key].FirstOrDefault()?.ToLowerInvariant())
                        return true;
                }
            }

            return false;
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
