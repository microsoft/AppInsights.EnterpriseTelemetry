using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;

namespace AppInsights.EnterpriseTelemetry.Client
{
    public interface IContextPropertyBuilder
    {
        ITelemetry Build(ITelemetry telemetry, Dictionary<string, string> propertyKeys);
        Dictionary<string, string> Build(Dictionary<string, string> propertyKeys);
        string GetProperty(string propertyKey);
    }
}