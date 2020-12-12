using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public class HttpContextPropertyBuilder : IContextPropertyBuilder
    {
        private IHttpContextAccessor _httpContextAccessor;

        public HttpContextPropertyBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContextPropertyBuilder()
            : this(new HttpContextAccessor())
        { }

        public ITelemetry Build(ITelemetry telemetry, Dictionary<string, string> propertyKeys)
        {   
            var contextProperties = Build(propertyKeys);
            ((ISupportProperties)telemetry).Properties.Copy(contextProperties);
            return telemetry;
        }

        public Dictionary<string, string> Build(Dictionary<string, string> propertyKeys)
        {
            var properties = new Dictionary<string, string>();
            if (propertyKeys == null || !propertyKeys.Any())
                return properties;

            foreach (var propertyKeyPair in propertyKeys)
            {
                properties.Add(propertyKeyPair.Key, GetProperty(propertyKeyPair.Value));
            }

            return properties;
        }

        public string GetProperty(string propertyKey)
        {
            var httpContext = GetHttpContext();
            if (httpContext == null)
                return null;

            var property = string.Empty;

            var headerProperty = httpContext.Request?.Headers?.FirstOrDefault(header => header.Key.ToLowerInvariant() == propertyKey.ToLowerInvariant());
            if (headerProperty != null)
            {
                property = (headerProperty.Value.Value.Count <= 1)
                    ? headerProperty.Value.Value.FirstOrDefault()
                    : string.Join(",", headerProperty.Value.Value);
            }

            if (!string.IsNullOrWhiteSpace(property))
                return property;

            var queryProperty = httpContext.Request?.Query?.FirstOrDefault(query => query.Key.ToLowerInvariant() == propertyKey.ToLowerInvariant());
            if (queryProperty != null)
            {
                property = (queryProperty.Value.Value.Count <= 1)
                    ? queryProperty.Value.Value.FirstOrDefault()
                    : string.Join(",", queryProperty.Value.Value);
            }

            if (!string.IsNullOrWhiteSpace(property))
                return property;

            return TelemetryConstant.NA;
        }

        private HttpContext GetHttpContext()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // Trying to re-initialize the HTTP Context accessor
                _httpContextAccessor = new HttpContextAccessor();
            }

            if (_httpContextAccessor == null)
                return null;

            return _httpContextAccessor.HttpContext;
        }

    }
}
