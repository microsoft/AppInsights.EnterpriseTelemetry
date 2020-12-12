using System.Collections.Generic;

namespace AppInsights.EnterpriseTelemetry.Context
{
    public class EventContextMetadata
    {
        public string Object { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Subject { get; set; }
        public string Desription { get; set; }
        public Dictionary<string, string> StaticProperties { get; set; }
        public Dictionary<string, string> ContextualProperties { get; set; }
    }
}
