using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using AppInsights.EnterpriseTelemetry.Configurations;

#pragma warning disable CA1031 // Do not catch general exception types
namespace AppInsights.EnterpriseTelemetry.Context
{
    /// <summary>
    /// Base Log Context for logging data
    /// </summary>
    public abstract class LogContext
    {
        public TraceLevel TraceLevel { get; set; }
        public string EndToEndTrackingId { get; set; }
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }
        public bool PropertySplittingEnabled { get; set; }
        public Dictionary<string, string> Properties { get; }
        public Dictionary<string, string> ContextualProperties { get; }

        /// <summary>
        /// Default Constructor for Log Context
        /// </summary>
        public LogContext()
        {
            Properties = new Dictionary<string, string>();
            ContextualProperties = new Dictionary<string, string>();
        }

        public LogContext(TraceLevel severityLevel, string correlationId, string transactionId, string source, string userId, string endToEndTrackingId)
        {
            TraceLevel = severityLevel;
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;
            TransactionId = string.IsNullOrWhiteSpace(transactionId) ? Guid.NewGuid().ToString() : transactionId;
            EndToEndTrackingId = string.IsNullOrWhiteSpace(endToEndTrackingId) ? "NOT AVAILABLE" : endToEndTrackingId;
            Source = source;
            UserId = userId;
            Properties = new Dictionary<string, string>()
            {
                { "XCV", CorrelationId },
                { "MessageId", TransactionId },
                { "TrackingId", EndToEndTrackingId },
                { "Source", source  },
                { "UserId", userId  }
            };
            ContextualProperties = new Dictionary<string, string>();
        }

        public void AddTrackingIds(string correlationId, string transactionId, string e2eTrackingId)
        {
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                CorrelationId = correlationId;
                AddProperty("XCV", correlationId, true);
            }
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                TransactionId = transactionId;
                AddProperty("MessageId", transactionId, true);
            }

            if (!string.IsNullOrWhiteSpace(e2eTrackingId))
            {
                EndToEndTrackingId = e2eTrackingId;
                AddProperty("TrackingId", e2eTrackingId, overridePrevious: true);
            }
        }

        public void AddUserId(string userId)
        {
            UserId = userId;
            AddProperty(nameof(UserId), userId, overridePrevious: true);
        }


        public void AddProperty(string propertyName, string propertyValue)
        {
            AddProperty(propertyName, propertyValue, overridePrevious: true);
        }

        public void AddProperty(string propertyName, object propertyValue)
        {
            try
            {
                var serializerPropertyValue = JsonSerializer.Serialize(propertyValue);
                AddProperty(propertyName, serializerPropertyValue, overridePrevious: true);
            }
            catch (Exception)
            {
                AddProperty(propertyName, "**UNREADABLE**", overridePrevious: true);
            }
        }

        public void AddProperty(string propertyName, string propertyValue, bool overridePrevious)
        {
            if (Properties.ContainsKey(propertyName))
            {
                if (overridePrevious)
                {
                    Properties[propertyName] = propertyValue;
                }
            }
            else
            {
                Properties[propertyName] = propertyValue;
            }
        }

        public void AddContextualProperty(string propertyName, string propertyValue)
        {
            ContextualProperties.AddOrUpdate(propertyName, propertyValue);
        }

        public void AddProperties(Dictionary<string, string> properties)
        {
            AddProperties(properties, overridePrevious: true);
        }

        public void AddProperties(Dictionary<string, string> properties, bool overridePrevious)
        {
            foreach (var property in properties)
            {
                AddProperty(property.Key, property.Value, overridePrevious);
            }
        }

        public void AddContextualProperties(Dictionary<string, string> contextualProperties)
        {
            ContextualProperties.Copy(contextualProperties);
        }

        public virtual void Trim(ApplicationInsightsConfiguration configuration)
        {
            try
            {
                if (!(PropertySplittingEnabled || configuration.PropertySplittingEnabled))
                    return;
                

                var violatedProperties = new List<string>();
                var newProperties = new List<KeyValuePair<string, string>>();
                foreach (var property in Properties)
                {
                    if (property.Value.Length > configuration.MaxMessageSize)
                    {
                        violatedProperties.Add(property.Key);
                        var splitPropertyValues = property.Value.Split(configuration.MaxPropertySize);
                        for (var index = 1; index <= splitPropertyValues.Count; index++)
                        {
                            newProperties.Add(new KeyValuePair<string, string>($"{property.Key}_{index}", splitPropertyValues[index - 1]));
                        }
                    }
                }

                if (!violatedProperties.Any())
                    return;

                Properties.Remove(violatedProperties);
                Properties.AddOrUpdate(newProperties);
            }
            catch (Exception exception)
            {
                Properties.Add("TrimFailure", exception.ToString());
            }
        }
    }
}
#pragma warning restore CA1031 // Do not catch general exception types
