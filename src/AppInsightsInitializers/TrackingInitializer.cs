using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using AppInsights.EnterpriseTelemetry.Configurations;
using AppInsights.EnterpriseTelemetry.Context.Background;
using static AppInsights.EnterpriseTelemetry.TelemetryConstant;
using System.Runtime.Serialization.Formatters;

namespace AppInsights.EnterpriseTelemetry.AppInsightsInitializers
{
    /// <summary>
    /// Adds tracking properties (correlation ID, logged-in user, etc) to all logs
    /// </summary>
    public class TrackingInitializer : ITelemetryInitializer
    {
        private readonly AppMetadataConfiguration _appConfiguration;
        private readonly ApplicationInsightsConfiguration _appInsightsConfiguration;
        private IHttpContextAccessor _httpContextAccessor;

        public TrackingInitializer(IHttpContextAccessor httpContextAccessor, ApplicationInsightsConfiguration appInsightsConfigurations, AppMetadataConfiguration appConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _appInsightsConfiguration = appInsightsConfigurations;
            _appConfiguration = appConfiguration;
        }

        public TrackingInitializer(ApplicationInsightsConfiguration appInsightsConfigurations, AppMetadataConfiguration configuration)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _appInsightsConfiguration = appInsightsConfigurations;
            _appConfiguration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                AddCorrelationId(telemetry, defaultValue: Guid.NewGuid().ToString());
                AddContextTrackingId(telemetry, _appInsightsConfiguration.TransactionIdPropertyKey, _appConfiguration.TransactionIdHeaderKey, NA);
                AddContextTrackingId(telemetry, _appInsightsConfiguration.SubCorrelationIdPropertyKey, _appConfiguration.SubCorrIdHeaderKey, NA);
                AddContextTrackingId(telemetry, _appInsightsConfiguration.EndToEndIdPropertyKey, _appConfiguration.EndToEndTrackingHeaderKey, NA);
                AddContextTrackingId(telemetry, _appInsightsConfiguration.TenantIdPropertyKey, _appConfiguration.TenantIdHeaderKey, NA);
                AddContextTrackingId(telemetry, _appInsightsConfiguration.BusinessProcessPropertyKey, _appConfiguration.BusinessProcessHeaderKey, NA);
                AddStaticProperties(telemetry, _appInsightsConfiguration.StaticProperties);
                UpdateSource(telemetry, _appInsightsConfiguration.TelemetrySource);

                if (_appInsightsConfiguration.CustomTrackingProperties.Any())
                {
                    foreach (var customProperty in _appInsightsConfiguration.CustomTrackingProperties)
                    {
                        AddContextTrackingId(telemetry, customProperty.Key, customProperty.Value, NA);
                    }
                }

                var userId = GetUserFromHttpContext();
                ((ISupportProperties)telemetry).Properties[_appInsightsConfiguration.UserPropertyKey] = userId;
                telemetry.Context.User.Id = userId;

                var bgUserId = GetTrackingIdFromBackgroundContext(telemetry, contextHeaderKey: "User");
                if (!string.IsNullOrWhiteSpace(bgUserId))
                {
                    ((ISupportProperties)telemetry).Properties["OriginalUserId"] = bgUserId;
                }
            }
            catch (Exception exception)
            {
                ((ISupportProperties)telemetry).Properties.Add("TRACKING_INITIALIZER_EXCEPTION", exception.ToString());
            }
        }

        private void AddCorrelationId(ITelemetry telemetry, string defaultValue)
        {
            var telemetryCorrelationId = ((ISupportProperties)telemetry).Properties.GetOrDefault(_appInsightsConfiguration.CorrelationIdPropertyKey, string.Empty);
            var httpContextCorrelationId = GetTrackingIdFromHttpContext(_appConfiguration.CorrelationIdHeaderKey);

            if (!string.IsNullOrWhiteSpace(telemetryCorrelationId))
            {
                if (!string.IsNullOrWhiteSpace(httpContextCorrelationId) && telemetryCorrelationId != httpContextCorrelationId)
                {
                    ((ISupportProperties)telemetry).Properties[_appInsightsConfiguration.CorrelationIdPropertyKey] = httpContextCorrelationId;
                    ((ISupportProperties)telemetry).Properties[$"{_appInsightsConfiguration.CorrelationIdPropertyKey}:Alternate"] = telemetryCorrelationId;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(httpContextCorrelationId))
                {
                    ((ISupportProperties)telemetry).Properties[_appInsightsConfiguration.CorrelationIdPropertyKey] = httpContextCorrelationId;
                }
                else
                {
                    ((ISupportProperties)telemetry).Properties[_appInsightsConfiguration.CorrelationIdPropertyKey] = defaultValue;
                }
            }
        }

        private void AddContextTrackingId(ITelemetry telemetry, string logPropertyKey, string contextHeaderKey, string defaultValue)
        {
            var existingTrackingId = ((ISupportProperties)telemetry).Properties.GetOrDefault(logPropertyKey, string.Empty);
            var contextTrackingId = GetTrackingIdFromHttpContext(contextHeaderKey);
            contextTrackingId = string.IsNullOrWhiteSpace(contextTrackingId) ? GetTrackingIdFromBackgroundContext(telemetry, contextHeaderKey) : contextTrackingId;

            if (!string.IsNullOrWhiteSpace(existingTrackingId))
            {
                if (!string.IsNullOrWhiteSpace(contextTrackingId) && contextTrackingId != existingTrackingId)
                {
                    ((ISupportProperties)telemetry).Properties[logPropertyKey] = contextTrackingId;
                    ((ISupportProperties)telemetry).Properties[$"{logPropertyKey}:Alternate"] = existingTrackingId;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(contextTrackingId))
                {
                    ((ISupportProperties)telemetry).Properties[logPropertyKey] = contextTrackingId;
                }
                else
                {
                    ((ISupportProperties)telemetry).Properties[logPropertyKey] = defaultValue;
                }
            }
        }

        private void AddStaticProperties(ITelemetry telemetry, Dictionary<string, string> staticProperties)
        {
            if (staticProperties == null || !staticProperties.Any())
                return;

            foreach(KeyValuePair<string, string> staticProperty in staticProperties)
            {
                ((ISupportProperties)telemetry).Properties.Add(staticProperty.Key, staticProperty.Value);
            }
        }

        private void UpdateSource(ITelemetry telemetry, string sourcePrefix)
        {
            if (string.IsNullOrWhiteSpace(sourcePrefix))
                return;

            string source = ((ISupportProperties)telemetry).Properties.GetOrDefault("Source", "");
            source = !string.IsNullOrWhiteSpace(source) && !source.StartsWith(sourcePrefix) 
                ? $"{sourcePrefix}{source}" 
                : sourcePrefix;
            ((ISupportProperties)telemetry).Properties.AddOrUpdate("Source", source);
        }

        private string GetCorrelationId(ITelemetry telemetry)
        {
            if (_httpContextAccessor.HttpContext != null
                && _httpContextAccessor.HttpContext.Request != null
                && _httpContextAccessor.HttpContext.Request.Headers != null
                && _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(_appConfiguration.CorrelationIdHeaderKey)
                && !string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Request.Headers[_appConfiguration.CorrelationIdHeaderKey]))
            {
                return _httpContextAccessor.HttpContext.Request.Headers.GetOrDefault(_appConfiguration.CorrelationIdHeaderKey, string.Empty);
            }

            if (((ISupportProperties)telemetry).Properties.ContainsKey(_appInsightsConfiguration.CorrelationIdPropertyKey))
            {
                return ((ISupportProperties)telemetry).Properties[_appInsightsConfiguration.CorrelationIdPropertyKey];
            }
            return string.Empty;
        }

        private string GetTrackingIdFromHttpContext(string contextHeaderKey)
        {
            if (!string.IsNullOrWhiteSpace(contextHeaderKey)
                && _httpContextAccessor.HttpContext != null
                && _httpContextAccessor.HttpContext.Request != null
                && _httpContextAccessor.HttpContext.Request.Headers != null
                && _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(contextHeaderKey))
            {
                return _httpContextAccessor.HttpContext.Request.Headers.GetOrDefault(contextHeaderKey, string.Empty);
            }
            return string.Empty;
        }

        private string GetTrackingIdFromBackgroundContext(ITelemetry telemetry, string contextHeaderKey)
        {
            var correlationId = GetCorrelationId(telemetry);
            var backgroundContext = BackgroundContext.GetCurrentContextByCorrelationId(correlationId);
            if (backgroundContext == null)
                return string.Empty;

            if (contextHeaderKey == _appConfiguration.TransactionIdHeaderKey)
                return backgroundContext.TransactionId;
            if (contextHeaderKey == _appConfiguration.EndToEndTrackingHeaderKey)
                return backgroundContext.EndToEndTrackingId;
            if (contextHeaderKey == _appConfiguration.TenantIdHeaderKey)
                return backgroundContext.TenantId;
            return backgroundContext.UserId;
        }

        private string GetUserFromHttpContext()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // Trying to re-initialize the HTTP Context accessor
                _httpContextAccessor = new HttpContextAccessor();
            }

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null && _httpContextAccessor.HttpContext.Request.Headers != null
                && _httpContextAccessor.HttpContext.User != null)
            {
                if (!string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.User.Identity.Name))
                {
                    return _httpContextAccessor.HttpContext.User.Identity.Name;
                }
                else
                {
                    var appIdClaims = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "appid");
                    if (appIdClaims != null)
                        return $"SPN:{appIdClaims.Value}";
                }
            }
            return null;
        }
    }
}
